using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 用户角色关联实体
/// </summary>
[SugarTable("sys_user_role", "用户角色关联表")]
/// <summary>
/// SysUserRole
/// </summary>
public class SysUserRole : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }
}
