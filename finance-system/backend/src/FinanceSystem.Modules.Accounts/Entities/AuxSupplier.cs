using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 辅助核算-供应商实体
/// </summary>
[SugarTable("fm_aux_supplier", "辅助核算供应商表")]
public class AuxSupplier : BaseEntity
{
    /// <summary>
    /// 供应商编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string SupplierCode { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string SupplierName { get; set; } = string.Empty;

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
    /// 开户银行
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? BankAccount { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;
}
