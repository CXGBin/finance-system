using FinanceSystem.Core.Common;
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
        // TODO: 从JWT中获取当前用户ID和Token
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _authService.LogoutAsync(0, "");
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
        // TODO: 从JWT中获取当前用户ID
        await _authService.ChangePasswordAsync(0, request);
        return ApiResult<bool>.Success(true);
    }
}
