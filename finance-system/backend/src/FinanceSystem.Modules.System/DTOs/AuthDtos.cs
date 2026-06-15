namespace FinanceSystem.Modules.System.DTOs;

/// <summary>
/// 登录请求参数
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 记住我（延长refresh_token有效期）
    /// </summary>
    public bool RememberMe { get; set; }
}

/// <summary>
/// 登录响应数据
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 访问令牌有效期（秒）
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 用户基本信息
    /// </summary>
    public UserInfoDto UserInfo { get; set; } = new();
}

/// <summary>
/// Token刷新请求
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// 旧的刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 修改密码请求
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 旧密码
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 用户基本信息（登录后返回）
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 权限标识列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// 已启用模块列表
    /// </summary>
    public List<string> Modules { get; set; } = new();
}
