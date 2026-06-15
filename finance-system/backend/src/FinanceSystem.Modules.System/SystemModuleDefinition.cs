using FinanceSystem.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Modules.System;

/// <summary>
/// 系统管理模块定义（核心模块，始终启用）
/// </summary>
public class SystemModuleDefinition : IModuleDefinition
{
    public string ModuleId => "system";
    public string ModuleName => "系统管理";
    public string Description => "用户、角色、菜单、部门、岗位、字典、日志、模块管理、系统配置";
    public bool IsCore => true;
    public string[] Dependencies => Array.Empty<string>();

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<Services.IAuthService, Services.AuthService>();
        services.AddScoped<Services.IUserService, Services.UserService>();
        services.AddScoped<Services.IRoleService, Services.RoleService>();
        services.AddScoped<Services.IMenuService, Services.MenuService>();
        services.AddScoped<Services.IDeptService, Services.DeptService>();
        services.AddScoped<Services.IPostService, Services.PostService>();
        services.AddScoped<Services.IDictService, Services.DictService>();
        services.AddScoped<Services.ILogService, Services.LogService>();
        services.AddScoped<Services.IModuleService, Services.ModuleService>();
        services.AddScoped<Services.IConfigService, Services.ConfigService>();
        services.AddScoped<Services.INoticeService, Services.NoticeService>();
    }
}
