using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统角色实体
/// </summary>
[SugarTable("sys_role", "系统角色表")]
public class SysRole : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（唯一，大写字母+下划线）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态（0禁用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 角色关联菜单ID列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<long>? MenuIds { get; set; }
}
