using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 角色菜单关联实体
/// </summary>
[SugarTable("sys_role_menu", "角色菜单关联表")]
/// <summary>
/// SysRoleMenu
/// </summary>
public class SysRoleMenu : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; }
}
