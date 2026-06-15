using FinanceSystem.Core.Common;
using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.System.Services;
/// <summary>
/// 认证服务（登录/登出/刷新Token/修改密码）
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求参数</param>
    /// <param name="ip">客户端IP</param>
    /// <returns>登录响应数据（含Token和用户信息）</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request, string ip);
    /// 用户登出（使Token失效）
    /// <param name="userId">当前用户ID</param>
    /// <param name="accessToken">当前访问令牌</param>
    Task LogoutAsync(long userId, string accessToken);
    /// 刷新Token
    /// <param name="request">刷新Token请求</param>
    /// <returns>新的登录响应数据</returns>
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    /// 修改密码
    /// <param name="request">修改密码请求</param>
    Task ChangePasswordAsync(long userId, ChangePasswordRequest request);
}
