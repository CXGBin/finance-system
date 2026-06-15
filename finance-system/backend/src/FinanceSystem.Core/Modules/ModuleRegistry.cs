using FinanceSystem.Core.Interfaces;

namespace FinanceSystem.Core.Modules;

/// <summary>
/// 模块注册器实现（管理所有业务模块的注册、查询与开关状态）
/// </summary>
public class ModuleRegistry : IModuleRegistry
{
    /// <summary>已注册的模块定义（ModuleId → 定义）</summary>
    private readonly Dictionary<string, IModuleDefinition> _modules = new();

    /// <summary>模块开关配置（ModuleId → 是否启用），null 表示未配置（默认启用）</summary>
    private readonly Dictionary<string, bool?> _switches = new();

    /// <summary>
    /// 注册一个模块定义
    /// </summary>
    public void RegisterModule(IModuleDefinition module)
    {
        ArgumentNullException.ThrowIfNull(module);
        _modules[module.ModuleId] = module;
    }

    /// <summary>
    /// 批量注册模块定义
    /// </summary>
    public void RegisterModules(IEnumerable<IModuleDefinition> modules)
    {
        foreach (var module in modules)
            RegisterModule(module);
    }

    /// <summary>
    /// 配置模块开关状态
    /// </summary>
    /// <param name="moduleId">模块标识</param>
    /// <param name="enabled">是否启用</param>
    public void SetModuleEnabled(string moduleId, bool enabled)
    {
        _switches[moduleId] = enabled;
    }

    /// <summary>
    /// 批量配置模块开关（从字典读取，未出现的模块使用默认值）
    /// </summary>
    public void ApplySwitches(IDictionary<string, bool> switches)
    {
        foreach (var kv in switches)
            _switches[kv.Key] = kv.Value;
    }

    /// <summary>
    /// 获取所有已注册的模块定义
    /// </summary>
    public IEnumerable<IModuleDefinition> GetAllModules() => _modules.Values;

    /// <summary>
    /// 根据模块ID获取模块定义
    /// </summary>
    public IModuleDefinition? GetModule(string moduleId)
        => _modules.GetValueOrDefault(moduleId);

    /// <summary>
    /// 检查模块是否启用
    /// 核心模块始终启用，非核心模块默认启用（除非显式关闭）
    /// </summary>
    public bool IsModuleEnabled(string moduleId)
    {
        if (!_modules.TryGetValue(moduleId, out var module))
            return true; // 未知模块默认放行

        if (module.IsCore)
            return true; // 核心模块始终启用

        if (_switches.TryGetValue(moduleId, out var enabled))
            return enabled.Value; // 显式配置

        return true; // 非核心模块默认启用
    }
}
