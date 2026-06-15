using FinanceSystem.Core.Interfaces;
using FinanceSystem.Modules.Approval.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.Approval;

/// <summary>
/// 审批流程模块定义
/// </summary>
/// <summary>
/// ApprovalModuleDefinition
/// </summary>
public class ApprovalModuleDefinition : IModuleDefinition
{
    public string ModuleId => "approval";
    public string ModuleName => "审批流程";
    public string Description => "流程定义、审批实例、审批记录、撤回";
    public bool IsCore => false;
    public string[] Dependencies => Array.Empty<string>();

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IApprovalFlowService, ApprovalFlowService>();
        services.AddScoped<IApprovalInstanceService, ApprovalInstanceService>();
    }
}
