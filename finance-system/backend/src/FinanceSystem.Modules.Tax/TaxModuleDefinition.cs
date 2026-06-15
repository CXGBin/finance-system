using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Tax.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Tax;

/// <summary>
/// 税务管理模块定义
/// </summary>
/// <summary>
/// TaxModuleDefinition
/// </summary>
public class TaxModuleDefinition : IModuleDefinition
{
    public string ModuleId => "tax";
    public string ModuleName => "税务管理";
    public string Description => "税种配置、纳税申报、发票登记（进项/销项）、认证";
    public bool IsCore => false;
    public string[] Dependencies => new[] { "account" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ITaxCategoryService, TaxCategoryService>();
        services.AddScoped<ITaxDeclarationService, TaxDeclarationService>();
        services.AddScoped<ITaxInvoiceService, TaxInvoiceService>();
        services.AddScoped<ITaxCalendarService, TaxCalendarService>();
        services.AddScoped<ITaxReportService, TaxReportService>();
    }
}
