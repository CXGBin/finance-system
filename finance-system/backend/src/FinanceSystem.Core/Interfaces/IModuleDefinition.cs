using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Core.Interfaces;

/// <summary>
/// 模块定义接口（每个业务模块实现此接口）
/// </summary>
public interface IModuleDefinition
{
    /// <summary>
    /// 模块标识（唯一，如 system、account、report）
    /// </summary>
    string ModuleId { get; }

    /// <summary>
    /// 模块名称（如 系统管理、账务管理）
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// 模块描述
    /// </summary>
    string Description { get; }

    /// <summary>
    /// 是否为核心模块（核心模块不可关闭）
    /// </summary>
    bool IsCore { get; }

    /// <summary>
    /// 依赖的模块ID列表
    /// </summary>
    string[] Dependencies { get; }

    /// <summary>
    /// 注册模块服务到DI容器
    /// </summary>
    /// <param name="services">服务集合</param>
    void RegisterServices(IServiceCollection services);
}

/// <summary>
/// 模块注册器（管理所有模块的注册与查询）
/// </summary>
public interface IModuleRegistry
{
    /// <summary>
    /// 注册一个模块定义
    /// </summary>
    /// <param name="module">模块定义实例</param>
    void RegisterModule(IModuleDefinition module);

    /// <summary>
    /// 获取所有已注册的模块定义
    /// </summary>
    /// <returns>模块定义列表</returns>
    IEnumerable<IModuleDefinition> GetAllModules();

    /// <summary>
    /// 根据模块ID获取模块定义
    /// </summary>
    /// <param name="moduleId">模块标识</param>
    /// <returns>模块定义，未找到返回null</returns>
    IModuleDefinition? GetModule(string moduleId);

    /// <summary>
    /// 根据配置检查模块是否启用
    /// </summary>
    /// <param name="moduleId">模块标识</param>
    /// <returns>是否启用</returns>
    bool IsModuleEnabled(string moduleId);
}
