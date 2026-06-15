using FinanceSystem.Core.Common;
using SqlSugar;
using System.Text.Json;

namespace FinanceSystem.Api.Middleware;

/// <summary>
/// 模块开关校验中间件
/// 拦截已关闭模块的API请求，返回403错误
/// </summary>
/// <summary>
/// ModuleSwitchMiddleware
/// </summary>
public class ModuleSwitchMiddleware
{
    /// <summary>
    /// 下一个中间件委托
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// 模块状态缓存（模块ID → 是否启用）
    /// </summary>
    private static Dictionary<string, bool>? _moduleCache;

    /// <summary>
    /// 缓存过期时间
    /// </summary>
    private static DateTime _cacheExpireTime = DateTime.MinValue;

    /// <summary>
    /// 缓存有效时长（分钟）
    /// </summary>
    private const int CacheDurationMinutes = 5;

    /// <summary>
    /// 创建模块开关中间件
    /// </summary>
    /// <param name="next">下一个中间件委托</param>
    public ModuleSwitchMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 处理请求，校验模块是否启用
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            // 从路径提取模块标识：/api/{module}/...
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2)
            {
                var moduleId = segments[1].ToLower();
                // 系统管理模块是核心模块，始终放行
                if (moduleId != "system" && moduleId != "auth")
                {
                    var isEnabled = await IsModuleEnabledAsync(context, moduleId);
                    if (!isEnabled)
                    {
                        // 模块未启用，返回403
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = ApiResult.Fail("该功能模块未启用", 403);
                        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        await context.Response.WriteAsync(json);
                        return;
                    }
                }
            }
        }

        await _next(context);
    }

    /// <summary>
    /// 检查模块是否启用（带缓存）
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="moduleId">模块标识</param>
    /// <returns>是否启用</returns>
    private static async Task<bool> IsModuleEnabledAsync(HttpContext context, string moduleId)
    {
        // 检查缓存是否有效
        if (_moduleCache != null && DateTime.Now < _cacheExpireTime)
        {
            if (_moduleCache.TryGetValue(moduleId, out var enabled))
            {
                return enabled;
            }
            // 缓存中找不到该模块，说明不存在，默认允许（避免拦截未知路由）
            return true;
        }

        // 缓存过期，重新从数据库加载
        await RefreshCacheAsync(context);

        if (_moduleCache != null && _moduleCache.TryGetValue(moduleId, out var isEnabled))
        {
            return isEnabled;
        }

        // 找不到模块定义，默认放行
        return true;
    }

    /// <summary>
    /// 从数据库刷新模块状态缓存
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    private static async Task RefreshCacheAsync(HttpContext context)
    {
        try
        {
            var db = context.RequestServices.GetRequiredService<ISqlSugarClient>();
            var modules = await db.Queryable<FinanceSystem.Modules.System.Entities.SysModule>()
                .ToListAsync();

            _moduleCache = modules.ToDictionary(m => m.ModuleId.ToLower(), m => m.IsEnabled == 1);
            _cacheExpireTime = DateTime.Now.AddMinutes(CacheDurationMinutes);
        }
        catch
        {
            // 数据库不可用时，允许所有请求通过（避免阻塞启动）
            _moduleCache ??= new Dictionary<string, bool>();
            _cacheExpireTime = DateTime.Now.AddMinutes(1);
        }
    }

    /// <summary>
    /// 清除缓存（供模块开关变更时调用）
    /// </summary>
    public static void ClearCache()
    {
        _moduleCache = null;
        _cacheExpireTime = DateTime.MinValue;
    }
}
