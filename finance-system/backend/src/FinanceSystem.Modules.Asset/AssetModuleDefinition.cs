using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Asset.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Asset;

/// <summary>
/// 资产管理模块定义
/// </summary>
/// <summary>
/// AssetModuleDefinition
/// </summary>
public class AssetModuleDefinition : IModuleDefinition
{
    public string ModuleId => "asset";
    public string ModuleName => "资产管理";
    public string Description => "资产分类、资产卡片、折旧计算（直线法/双倍余额/年数总和）、资产变动、处置";
    public bool IsCore => false;
    public string[] Dependencies => new[] { "account" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAssetCategoryService, AssetCategoryService>();
        services.AddScoped<IAssetCardService, AssetCardService>();
        services.AddScoped<IAssetDepreciationService, AssetDepreciationService>();
        services.AddScoped<IAssetInventoryService, AssetInventoryService>();
    }
}
