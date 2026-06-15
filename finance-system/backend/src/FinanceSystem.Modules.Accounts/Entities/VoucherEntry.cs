using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 凭证分录实体
/// </summary>
[SugarTable("fm_voucher_entry", "凭证分录表")]
/// <summary>
/// VoucherEntry
/// </summary>
public class VoucherEntry : BaseEntity
{
    /// <summary>
    /// 凭证ID
    /// </summary>
    public long VoucherId { get; set; }

    /// <summary>
    /// 分录摘要
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Summary { get; set; }

    /// <summary>
    /// 科目ID（末级科目）
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 借方金额
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 贷方金额
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 辅助核算ID（科目有辅助核算时填写）
    /// </summary>
    public long? AuxiliaryId { get; set; }

    /// <summary>
    /// 辅助核算类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? AuxiliaryType { get; set; }

    /// <summary>
    /// 科目信息（导航属性）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public AccountSubject? Subject { get; set; }
}
