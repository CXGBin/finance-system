using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Accounts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Accounts;

/// <summary>
/// 账务管理模块定义（核心模块，始终启用）
/// </summary>
/// <summary>
/// AccountsModuleDefinition
/// </summary>
public class AccountsModuleDefinition : IModuleDefinition
{
    public string ModuleId => "account";
    public string ModuleName => "账务管理";
    public string Description => "科目管理、凭证管理、会计期间、期初余额、总账明细账日记账、辅助核算";
    public bool IsCore => true;
    public string[] Dependencies => new[] { "system" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IVoucherService, VoucherService>();
        services.AddScoped<IPeriodService, PeriodService>();
        services.AddScoped<ISubjectBalanceService, SubjectBalanceService>();
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<IAuxiliaryService, AuxiliaryService>();
    }
}
