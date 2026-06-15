using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 辅助核算-项目实体
/// </summary>
[SugarTable("fm_aux_project", "辅助核算项目表")]
public class AuxProject : BaseEntity
{
    /// <summary>
    /// 项目编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 项目名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// 项目负责人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Manager { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;
}
