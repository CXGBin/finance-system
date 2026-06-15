using System.Text.Json;
using FinanceSystem.Core.Common;

namespace FinanceSystem.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
/// <summary>
/// GlobalExceptionMiddleware
/// </summary>
public class GlobalExceptionMiddleware
{
    /// <summary>
    /// 下一个中间件委托
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// 创建全局异常处理中间件
    /// </summary>
    /// <param name="next">下一个中间件委托</param>
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 处理请求并捕获异常
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            await HandleBusinessException(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleException(context, ex.Message, 401);
        }
        catch (Exception)
        {
            await HandleException(context, "服务器内部错误", 500);
        }
    }

    /// <summary>
    /// 处理业务异常
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="ex">业务异常</param>
    private static async Task HandleBusinessException(HttpContext context, BusinessException ex)
    {
        await HandleException(context, ex.Message, ex.Code);
    }

    /// <summary>
    /// 写入统一错误响应
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    private static async Task HandleException(HttpContext context, string message, int code)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";

        var result = ApiResult.Fail(message, code);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
