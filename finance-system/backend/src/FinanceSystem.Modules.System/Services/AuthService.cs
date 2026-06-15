using Microsoft.Extensions.Configuration;
using FinanceSystem.Core.Common;
using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;

    public AuthService(ISqlSugarClient db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> LoginAsync(LoginRequest request, string ip)
    {
        // 查询用户
        var user = await _db.Queryable<SysUser>()
            .FirstAsync(u => u.Username == request.Username)
            ?? throw new UnauthorizedException("用户名或密码错误");

        // 检查账户锁定状态
        if (user.LockoutEndTime.HasValue && user.LockoutEndTime.Value > DateTime.Now)
        {
            throw new BusinessException($"账户已锁定，请{Math.Ceiling((user.LockoutEndTime.Value - DateTime.Now).TotalMinutes)}分钟后再试");
        }

        // 验证密码（BCrypt）
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            // 累计登录失败次数
            user.LoginFailCount++;
            if (user.LoginFailCount >= 5)
            {
                user.LockoutEndTime = DateTime.Now.AddMinutes(30);
                user.LoginFailCount = 0;
            }
            await _db.Updateable(user).UpdateColumns(u => new { u.LoginFailCount, u.LockoutEndTime }).ExecuteCommandAsync();
            throw new UnauthorizedException("用户名或密码错误");
        }

        // 检查用户状态
        if (user.Status == 0)
        {
            throw new BusinessException("账户已被停用，请联系管理员");
        }

        // 登录成功，重置失败计数
        user.LoginFailCount = 0;
        user.LockoutEndTime = null;
        await _db.Updateable(user).UpdateColumns(u => new { u.LoginFailCount, u.LockoutEndTime }).ExecuteCommandAsync();

        // 生成Token
        var accessToken = GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresIn = int.Parse(_config["Jwt:AccessTokenExpireMinutes"] ?? "120");

        var userInfo = await BuildUserInfoAsync(user);

        // 记录登录日志
        await SaveLogAsync(user.Id, user.RealName, "system", "LOGIN", "用户登录", ip);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn * 60,
            UserInfo = userInfo
        };
    }

    /// <inheritdoc/>
    public async Task LogoutAsync(long userId, string accessToken)
    {
        // TODO: 将Token加入Redis黑名单
        // 记录登出日志
        await SaveLogAsync(userId, null, "system", "LOGOUT", "用户登出", null);
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // TODO: 从Redis验证refresh_token有效性
        // 简化实现：直接生成新Token
        // 生产环境应在Redis中存储refresh_token并验证
        throw new NotImplementedException("Token刷新功能待Redis集成后实现");
    }

    /// <inheritdoc/>
    public async Task ChangePasswordAsync(long userId, ChangePasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new BusinessException("两次输入的新密码不一致");
        }

        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == userId)
            ?? throw new NotFoundException("用户不存在");

        // 验证旧密码
        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
        {
            throw new BusinessException("旧密码不正确");
        }

        // 生成新密码哈希
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedTime = DateTime.Now;
        await _db.Updateable(user).UpdateColumns(u => new { u.PasswordHash, u.UpdatedTime }).ExecuteCommandAsync();

        // TODO: 使当前Token失效
    }

    /// <summary>
    /// 生成JWT访问令牌
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>JWT令牌字符串</returns>
    private string GenerateToken(SysUser user)
    {
        var key = _config["Jwt:SecurityKey"] ?? "FinanceSystem2026SecretKey";
        var expireMinutes = int.Parse(_config["Jwt:AccessTokenExpireMinutes"] ?? "120");
        var issuer = _config["Jwt:Issuer"] ?? "FinanceSystem";
        var audience = _config["Jwt:Audience"] ?? "FinanceSystem";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.RealName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌（随机字符串）
    /// </summary>
    /// <returns>刷新令牌字符串</returns>
    private string GenerateRefreshToken()
    {
        // TODO: 存入Redis，生产环境应使用安全的随机数生成器
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 构建用户信息（含角色、权限、模块）
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>用户信息DTO</returns>
    private async Task<UserInfoDto> BuildUserInfoAsync(SysUser user)
    {
        // 查询用户角色
        var roleIds = await _db.Queryable<SysUserRole>()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var roles = await _db.Queryable<SysRole>()
            .Where(r => roleIds.Contains(r.Id) && r.Status == 1)
            .ToListAsync();

        // 查询角色关联的菜单权限
        var menuIds = await _db.Queryable<SysRoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .ToListAsync();

        var permissions = await _db.Queryable<SysMenu>()
            .Where(m => menuIds.Contains(m.Id) && m.Status == 1 && m.MenuType == 3)
            .Select(m => m.Permission!)
            .Where(p => p != null)
            .ToListAsync();

        // 查询已启用模块
        var modules = await _db.Queryable<SysModule>()
            .Where(m => m.IsEnabled == 1)
            .Select(m => m.ModuleId)
            .ToListAsync();

        return new UserInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Roles = roles.Select(r => r.RoleCode).ToList(),
            Permissions = permissions,
            Modules = modules
        };
    }

    /// <summary>
    /// 保存操作日志
    /// </summary>
    /// <param name="userId">操作人ID</param>
    /// <param name="userName">操作人姓名</param>
    /// <param name="module">所属模块</param>
    /// <param name="action">操作类型</param>
    /// <param name="description">操作描述</param>
    /// <param name="ip">IP地址</param>
    private async Task SaveLogAsync(long userId, string? userName, string module, string action, string description, string? ip)
    {
        var log = new SysLog
        {
            UserId = userId,
            UserName = userName,
            Module = module,
            Action = action,
            Description = description,
            IpAddress = ip
        };
        await _db.Insertable(log).ExecuteCommandAsync();
    }
}
