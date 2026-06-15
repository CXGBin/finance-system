using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Reports.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Reports;

/// <summary>
/// 报表中心模块定义
/// </summary>
public class ReportsModuleDefinition : IModuleDefinition
{
    public string ModuleId => "report";
    public string ModuleName => "报表中心";
    public string Description => "资产负债表、利润表、现金流量表、科目余额表、自定义报表、多期对比、导出";
    public bool IsCore => false;
    public string[] Dependencies => new[] { "account" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IBalanceSheetService, BalanceSheetService>();
        services.AddScoped<IIncomeStatementService, IncomeStatementService>();
        services.AddScoped<ICashFlowService, CashFlowService>();
        services.AddScoped<ISubjectBalanceReportService, SubjectBalanceReportService>();
        services.AddScoped<ICustomReportService, CustomReportService>();
        services.AddScoped<IReportExportService, ReportExportService>();
        services.AddScoped<ICompareService, CompareService>();
    }
}
