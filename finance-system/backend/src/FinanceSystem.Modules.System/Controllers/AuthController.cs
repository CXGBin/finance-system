using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 认证控制器（登录/登出/刷新Token/修改密码）
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    public async Task<ApiResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(request, ip);
        return ApiResult<LoginResponse>.Success(result);
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    [HttpPost("logout")]
    public async Task<ApiResult<bool>> Logout()
    {
        var userId = HttpContext.GetCurrentUserId();
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _authService.LogoutAsync(userId, token);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ApiResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return ApiResult<LoginResponse>.Success(result);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    [HttpPut("password")]
    public async Task<ApiResult<bool>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _authService.ChangePasswordAsync(HttpContext.GetCurrentUserId(), request);
        return ApiResult<bool>.Success(true);
    }
}
