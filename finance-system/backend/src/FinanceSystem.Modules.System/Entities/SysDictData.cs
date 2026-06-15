using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 数据字典项实体
/// </summary>
[SugarTable("sys_dict_data", "数据字典项表")]
/// <summary>
/// SysDictData
/// </summary>
public class SysDictData : BaseEntity
{
    /// <summary>
    /// 所属字典类型编码
    /// </summary>
    [SugarColumn(Length = 100)]
    /// <summary>
    /// 字典类型编码
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典标签（显示值）
    /// </summary>
    [SugarColumn(Length = 100)]
    /// <summary>
    /// 字典项标签
    /// </summary>
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典值（实际值）
    /// </summary>
    [SugarColumn(Length = 100)]
    /// <summary>
    /// 字典项值
    /// </summary>
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
