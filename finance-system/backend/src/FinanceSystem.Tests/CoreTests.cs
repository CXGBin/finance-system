using FinanceSystem.Core.Common;
using FinanceSystem.Core.Interfaces;
using FinanceSystem.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FinanceSystem.Tests.Core;

/// <summary>
/// 模块注册器单元测试
/// </summary>
/// <summary>
/// ModuleRegistry测试
/// </summary>
public class ModuleRegistryTests
{
    /// <summary>测试注册模块</summary>
    [Fact]
    public void RegisterModule_ShouldAddModule()
    {
        var registry = new ModuleRegistry();
        var module = new TestModule("test", "测试模块", false);

        registry.RegisterModule(module);

        var result = registry.GetModule("test");
        Assert.NotNull(result);
        Assert.Equal("测试模块", result!.ModuleName);
    }

    /// <summary>测试批量注册</summary>
    [Fact]
    public void RegisterModules_ShouldAddAll()
    {
        var registry = new ModuleRegistry();
        var modules = new IModuleDefinition[]
        {
            new TestModule("a", "A", false),
            new TestModule("b", "B", true),
        };

        registry.RegisterModules(modules);

        Assert.Equal(2, registry.GetAllModules().Count());
    }

    /// <summary>测试核心模块始终启用</summary>
    [Fact]
    public void IsModuleEnabled_CoreModule_ShouldAlwaysBeTrue()
    {
        var registry = new ModuleRegistry();
        registry.RegisterModule(new TestModule("core", "核心", isCore: true));
        registry.SetModuleEnabled("core", false);

        Assert.True(registry.IsModuleEnabled("core"));
    }

    /// <summary>测试非核心模块默认启用</summary>
    [Fact]
    public void IsModuleEnabled_NonCoreDefault_ShouldBeTrue()
    {
        var registry = new ModuleRegistry();
        registry.RegisterModule(new TestModule("normal", "普通", false));

        Assert.True(registry.IsModuleEnabled("normal"));
    }

    /// <summary>测试非核心模块可关闭</summary>
    [Fact]
    public void IsModuleEnabled_NonCoreDisabled_ShouldBeFalse()
    {
        var registry = new ModuleRegistry();
        registry.RegisterModule(new TestModule("normal", "普通", false));
        registry.SetModuleEnabled("normal", false);

        Assert.False(registry.IsModuleEnabled("normal"));
    }

    /// <summary>测试未知模块默认放行</summary>
    [Fact]
    public void IsModuleEnabled_UnknownModule_ShouldBeTrue()
    {
        var registry = new ModuleRegistry();
        Assert.True(registry.IsModuleEnabled("nonexistent"));
    }

    /// <summary>测试获取不存在的模块</summary>
    [Fact]
    public void GetModule_NotFound_ShouldReturnNull()
    {
        var registry = new ModuleRegistry();
        Assert.Null(registry.GetModule("notexist"));
    }

    /// <summary>测试模块开关覆盖</summary>
    [Fact]
    public void SetModuleEnabled_Override_ShouldWork()
    {
        var registry = new ModuleRegistry();
        registry.RegisterModule(new TestModule("m", "M", false));

        registry.SetModuleEnabled("m", false);
        Assert.False(registry.IsModuleEnabled("m"));

        registry.SetModuleEnabled("m", true);
        Assert.True(registry.IsModuleEnabled("m"));
    }

    private class TestModule : IModuleDefinition
    {
        public string ModuleId { get; }
        public string ModuleName { get; }
        public string Description { get; } = "";
        public bool IsCore { get; }
        public string[] Dependencies { get; } = Array.Empty<string>();
        public TestModule(string id, string name, bool isCore) { ModuleId = id; ModuleName = name; IsCore = isCore; }
        public void RegisterServices(IServiceCollection services) { }
    }
}

/// <summary>
/// ApiResult 单元测试
/// </summary>
/// <summary>
/// ApiResult测试
/// </summary>
public class ApiResultTests
{
    [Fact]
    public void Success_ShouldReturn200()
    {
        var result = ApiResult.Success("ok");
        Assert.Equal(200, result.Code);
        Assert.Equal("ok", result.Message);
    }

    [Fact]
    public void Fail_ShouldReturnErrorCode()
    {
        var result = ApiResult.Fail("error", 400);
        Assert.Equal(400, result.Code);
        Assert.Equal("error", result.Message);
    }
}
