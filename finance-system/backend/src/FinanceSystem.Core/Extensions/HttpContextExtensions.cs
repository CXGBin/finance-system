using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FinanceSystem.Core.Extensions;

/// <summary>
/// HttpContext 扩展方法，用于从JWT中获取当前用户信息
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// 获取当前登录用户ID
    /// </summary>
    public static long GetCurrentUserId(this HttpContext context)
    {
        var claim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && long.TryParse(claim.Value, out var userId))
        {
            return userId;
        }
        return 0;
    }

    /// <summary>
    /// 获取当前登录用户名
    /// </summary>
    public static string GetCurrentUsername(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "";
    }

    /// <summary>
    /// 获取当前登录用户真实姓名
    /// </summary>
    public static string GetCurrentRealName(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    public static string GetClientIp(this HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
        {
            return ip.Split(',')[0].Trim();
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? "";
    }
}
