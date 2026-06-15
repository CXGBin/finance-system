using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 辅助核算-客户实体
/// </summary>
[SugarTable("fm_aux_customer", "辅助核算客户表")]
public class AuxCustomer : BaseEntity
{
    /// <summary>
    /// 客户编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// 税号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? TaxNo { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;
}
