using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统模块管理实体
/// </summary>
[SugarTable("sys_module", "模块管理表")]
/// <summary>
/// SysModule
/// </summary>
public class SysModule : FullEntity
{
    /// <summary>
    /// 模块标识（如 system/accounts/reports）
    /// </summary>
    [SugarColumn(Length = 50)]
    /// <summary>
    /// 模块标识
    /// </summary>
    public string ModuleId { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    [SugarColumn(Length = 100)]
    /// <summary>
    /// 模块名称
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 模块描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用（0禁用 1启用）
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 是否核心模块（0否 1是，核心模块不可关闭）
    /// </summary>
    public int IsCore { get; set; } = 0;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 依赖模块ID，逗号分隔
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Dependencies { get; set; }
}
