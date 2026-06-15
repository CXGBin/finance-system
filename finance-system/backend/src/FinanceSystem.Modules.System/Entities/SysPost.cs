using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 岗位实体
/// </summary>
[SugarTable("sys_post", "系统岗位表")]
public class SysPost : FullEntity
{
    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 岗位编码（唯一）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string PostCode { get; set; } = string.Empty;

    /// <summary>
    /// 岗位名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态（0禁用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
