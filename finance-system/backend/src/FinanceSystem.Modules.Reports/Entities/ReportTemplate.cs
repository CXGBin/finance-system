using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Reports.Entities;

/// <summary>
/// 自定义报表模板
/// </summary>
[SugarTable("fm_report_template", "自定义报表模板表")]
/// <summary>
/// ReportTemplate
/// </summary>
public class ReportTemplate : FullEntity
{
    /// <summary>
    /// 报表名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "报表名称")]
    /// <summary>
    /// 报表名称
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 报表模板JSON数据（含行定义、公式等）
    /// </summary>
    [SugarColumn(ColumnDataType = "nvarchar(max)", IsNullable = false, ColumnDescription = "模板JSON数据")]
    /// <summary>
    /// 模板JSON数据
    /// </summary>
    public string TemplateData { get; set; } = "[]";

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建人ID")]
    /// <summary>
    /// 创建人ID
    /// </summary>
    public long CreatedBy { get; set; }
}
