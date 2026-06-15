using System.Text.Json;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Core.Interfaces;
using SqlSugar;

namespace FinanceSystem.Api.Middleware;

/// <summary>
/// 操作日志自动记录中间件
/// 记录所有POST/PUT/DELETE请求（不含登录登出）
/// </summary>
/// <summary>
/// OperationLogMiddleware
/// </summary>
public class OperationLogMiddleware
{
    private readonly RequestDelegate _next;

    public OperationLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISqlSugarClient db)
    {
        // 仅记录写操作
        if (context.Request.Method is "POST" or "PUT" or "DELETE")
        {
            var path = context.Request.Path.Value;
            var userId = context.GetCurrentUserId();

            // 跳过登录登出
            if (path != null && !path.Contains("/auth/login") && !path.Contains("/auth/logout"))
            {
                // 读取请求体（小body直接读取）
                string? body = null;
                if (context.Request.ContentLength != null && context.Request.ContentLength < 4096)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                try
                {
                    await _next(context);
                }
                finally
                {
                    // 记录操作日志（异步写入，不阻塞响应）
                    var statusCode = context.Response.StatusCode;
                    var module = ExtractModule(path);
                    var operation = context.Request.Method;

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await db.Insertable(new Modules.System.Entities.SysLog
                            {
                                Module = module,
                                Action = operation,
                                Description = $"{operation} {path}",
                                RequestUrl = path,
                                RequestMethod = operation,
                                ResponseCode = statusCode,
                                UserId = userId,
                                IpAddress = context.GetClientIp(),
                                RequestBody = body?.Length > 2000 ? body[..2000] : body
                            }).ExecuteCommandAsync();
                        }
                        catch { /* 日志写入失败不影响主流程 */ }
                    });
                }
                return;
            }
        }

        await _next(context);
    }

    /// <summary>
    /// 从URL提取模块名
    /// </summary>
    private static string ExtractModule(string? path)
    {
        if (string.IsNullOrEmpty(path)) return "unknown";
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length >= 2 && segments[0] == "api")
            return segments[1];
        return segments.Length > 0 ? segments[0] : "unknown";
    }
}
