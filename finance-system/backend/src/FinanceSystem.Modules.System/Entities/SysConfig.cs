using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统配置实体
/// </summary>
[SugarTable("sys_config", "系统配置表")]
public class SysConfig : BaseEntity
{
    /// <summary>
    /// 配置分组（如 basic/accounting/log）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string ConfigGroup { get; set; } = string.Empty;

    /// <summary>
    /// 配置键（唯一）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 配置值
    /// </summary>
    [SugarColumn(Length = 500)]
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 配置名称（中文）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
