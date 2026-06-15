using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 部门实体
/// </summary>
[SugarTable("sys_dept", "系统部门表")]
public class SysDept : BaseEntity
{
    /// <summary>
    /// 父级部门ID（0为顶级）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 子部门列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysDept>? Children { get; set; }
}
