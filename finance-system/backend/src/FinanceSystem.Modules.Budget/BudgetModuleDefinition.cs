using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Budget.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Budget;

/// <summary>
/// 预算管理模块定义
/// </summary>
/// <summary>
/// BudgetModuleDefinition
/// </summary>
public class BudgetModuleDefinition : IModuleDefinition
{
    public string ModuleId => "budget";
    public string ModuleName => "预算管理";
    public string Description => "年度预算、科目预算、月度预算、执行跟踪、预算调整、预警通知";
    public bool IsCore => false;
    public string[] Dependencies => new[] { "account" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IBudgetYearService, BudgetYearService>();
        services.AddScoped<IBudgetSubjectService, BudgetSubjectService>();
        services.AddScoped<IBudgetMonthlyService, BudgetMonthlyService>();
        services.AddScoped<IBudgetExecutionService, BudgetExecutionService>();
        services.AddScoped<IBudgetAdjustService, BudgetAdjustService>();
        services.AddScoped<IBudgetAlertService, BudgetAlertService>();
    }
}
