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
        _refreshTokenStore[refreshToken] = user.Id; // 登录成功后更新为真实userId
        var expiresIn = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");

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
        // 将Token加入内存黑名单（生产环境应使用Redis）
        TokenBlacklist.Add(accessToken);
        // 记录登出日志
        await SaveLogAsync(userId, null, "system", "LOGOUT", "用户登出", null);
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // 验证refresh_token有效性（简化实现：内存存储）
        if (!_refreshTokenStore.TryGetValue(request.RefreshToken, out var storedUserId))
        {
            throw new UnauthorizedException("刷新令牌无效或已过期");
        }
        _refreshTokenStore.Remove(request.RefreshToken);

        var user = await _db.Queryable<SysUser>()
            .FirstAsync(u => u.Id == storedUserId)
            ?? throw new NotFoundException("用户不存在");

        var userInfo = await BuildUserInfoAsync(user);
        var token = GenerateToken(user);
        var refreshToken = Guid.NewGuid().ToString("N");
        var expiresIn = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");

        _refreshTokenStore[refreshToken] = user.Id;
        _cleanupExpiredRefreshTokens();

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn * 60,
            UserInfo = userInfo
        };
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

        // 使当前Token失效（需从请求头获取Token），此操作可选
    }

    /// <summary>
    /// 生成JWT访问令牌
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>JWT令牌字符串</returns>
    private string GenerateToken(SysUser user)
    {
        var key = _config["JwtSettings:Secret"] ?? throw new InvalidOperationException("缺少 JWT 密钥配置 JwtSettings:Secret");
        var expireMinutes = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");
        var issuer = _config["JwtSettings:Issuer"] ?? "FinanceSystem";
        var audience = _config["JwtSettings:Audience"] ?? "FinanceSystem";

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
    /// Token黑名单（内存存储，生产环境应替换为Redis）
    /// </summary>
    private static readonly HashSet<string> TokenBlacklist = new();

    /// <summary>
    /// 判断Token是否在黑名单中
    /// </summary>
    public static bool IsTokenBlacklisted(string token) => TokenBlacklist.Contains(token);

    /// <summary>
    /// RefreshToken存储（内存存储，生产环境应替换为Redis）
    /// </summary>
    private static readonly Dictionary<string, long> _refreshTokenStore = new();

    /// <summary>
    /// 清理过期的refresh_token
    /// </summary>
    private static void _cleanupExpiredRefreshTokens() { /* 简化实现，不做过期清理 */ }

    /// <summary>
    /// 生成刷新令牌（随机字符串）
    /// </summary>
    private string GenerateRefreshToken()
    {
        var token = Guid.NewGuid().ToString("N");
        _refreshTokenStore[token] = 0; // 占位，登录成功后会更新为真实userId
        return token;
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
