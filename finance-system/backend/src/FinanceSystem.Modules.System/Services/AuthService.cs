using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FinanceSystem.Core.Common;
using FinanceSystem.Core.Interfaces;
using FinanceSystem.Infrastructure.Services;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 认证服务实现（登录/登出/刷新Token/修改密码）
/// </summary>
public class AuthService : IAuthService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;
    private readonly ITokenBlacklistService _tokenBlacklist;
    private readonly IRefreshTokenStoreService _refreshTokenStore;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// 构造认证服务
    /// </summary>
    /// <param name="db">数据库客户端</param>
    /// <param name="config">配置</param>
    /// <param name="tokenBlacklist">Token黑名单服务</param>
    /// <param name="refreshTokenStore">RefreshToken存储服务</param>
    /// <param name="logger">日志</param>
    public AuthService(
        ISqlSugarClient db,
        IConfiguration config,
        ITokenBlacklistService tokenBlacklist,
        IRefreshTokenStoreService refreshTokenStore,
        ILogger<AuthService> logger)
    {
        _db = db;
        _config = config;
        _tokenBlacklist = tokenBlacklist;
        _refreshTokenStore = refreshTokenStore;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求参数</param>
    /// <param name="ip">客户端IP</param>
    /// <returns>登录响应数据（含Token和用户信息）</returns>
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
        var pwHash = user.PasswordHash ?? "";
        var verifyResult = BCrypt.Net.BCrypt.Verify(request.Password, pwHash);
        if (!verifyResult)
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
        var refreshExpireDays = int.Parse(_config["JwtSettings:RefreshTokenExpireDays"] ?? "7");
        await _refreshTokenStore.StoreAsync(refreshToken, user.Id, refreshExpireDays);
        var expiresIn = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");

        var userInfo = await BuildUserInfoAsync(user);

        // 记录登录日志
        await SaveLogAsync(user.Id, user.RealName, "system", "LOGIN", "用户登录", ip);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn * 60,
            UserInfo = userInfo,
            MustChangePassword = user.MustChangePassword
        };
    }

    /// <summary>
    /// 用户登出（使Token失效）
    /// </summary>
    /// <param name="userId">当前用户ID</param>
    /// <param name="accessToken">当前访问令牌</param>
    public async Task LogoutAsync(long userId, string accessToken)
    {
        try
        {
            var expireMinutes = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");
            await _tokenBlacklist.AddAsync(accessToken, expireMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token加入黑名单失败");
        }

        // 记录登出日志
        await SaveLogAsync(userId, null, "system", "LOGOUT", "用户登出", null);
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="request">刷新Token请求</param>
    /// <returns>新的登录响应数据</returns>
    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // 验证refresh_token有效性
        var storedUserId = await _refreshTokenStore.GetUserIdAsync(request.RefreshToken);
        if (!storedUserId.HasValue)
        {
            throw new UnauthorizedException("刷新令牌无效或已过期");
        }
        await _refreshTokenStore.RemoveAsync(request.RefreshToken);

        var user = await _db.Queryable<SysUser>()
            .FirstAsync(u => u.Id == storedUserId.Value)
            ?? throw new NotFoundException("用户不存在");

        var userInfo = await BuildUserInfoAsync(user);
        var token = GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresIn = int.Parse(_config["JwtSettings:AccessTokenExpireMinutes"] ?? "120");
        var refreshExpireDays = int.Parse(_config["JwtSettings:RefreshTokenExpireDays"] ?? "7");

        await _refreshTokenStore.StoreAsync(refreshToken, user.Id, refreshExpireDays);

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn * 60,
            UserInfo = userInfo
        };
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="request">修改密码请求</param>
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
        user.MustChangePassword = false;
        user.UpdatedTime = DateTime.Now;
        await _db.Updateable(user).UpdateColumns(u => new { u.PasswordHash, u.MustChangePassword, u.UpdatedTime }).ExecuteCommandAsync();
    }

    /// <summary>
    /// 判断Token是否在黑名单中（异步方法，供中间件调用）
    /// </summary>
    /// <param name="tokenBlacklist">Token黑名单服务</param>
    /// <param name="token">要检查的Token</param>
    /// <returns>是否在黑名单中</returns>
    public static async Task<bool> IsTokenBlacklistedAsync(ITokenBlacklistService tokenBlacklist, string token)
    {
        return await tokenBlacklist.IsBlacklistedAsync(token);
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
    /// 生成刷新令牌（随机字符串）
    /// </summary>
    private string GenerateRefreshToken()
    {
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
            .Where(m => menuIds.Contains(m.Id) && m.Status == 1 && m.MenuType == 3 && m.Permission != null)
            .Select(m => m.Permission!)
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
