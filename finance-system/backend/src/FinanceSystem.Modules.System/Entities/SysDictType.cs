using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 数据字典类型实体
/// </summary>
[SugarTable("sys_dict_type", "数据字典类型表")]
public class SysDictType : BaseEntity
{
    /// <summary>
    /// 字典名称（如"凭证类型"）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型编码（唯一，如"voucher_type"）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string DictType { get; set; } = string.Empty;

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
