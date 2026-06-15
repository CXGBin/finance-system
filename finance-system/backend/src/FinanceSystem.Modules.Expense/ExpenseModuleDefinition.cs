using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Expense.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Expense;

/// <summary>
/// 费用管理模块定义
/// </summary>
/// <summary>
/// ExpenseModuleDefinition
/// </summary>
public class ExpenseModuleDefinition : IModuleDefinition
{
    public string ModuleId => "expense";
    public string ModuleName => "费用管理";
    public string Description => "费用类型、报销单（草稿→审批→付款）、费用统计";
    public bool IsCore => false;
    public string[] Dependencies => new[] { "system" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
        services.AddScoped<IExpenseClaimService, ExpenseClaimService>();
        services.AddScoped<IExpenseStatisticsService, ExpenseStatisticsService>();
        services.AddScoped<IExpenseAllocateService, ExpenseAllocateService>();
    }
}
