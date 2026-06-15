using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 会计凭证实体
/// </summary>
[SugarTable("fm_voucher", "会计凭证表")]
/// <summary>
/// Voucher
/// </summary>
public class Voucher : BaseEntity
{
    /// <summary>
    /// 凭证编号
    /// </summary>
    [SugarColumn(Length = 20)]
    /// <summary>
    /// 凭证号
    /// </summary>
    public string VoucherNo { get; set; } = string.Empty;

    /// <summary>
    /// 凭证日期
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 会计期间ID
    /// </summary>
    public long PeriodId { get; set; }

    /// <summary>
    /// 凭证类型（1收款 2付款 3转账）
    /// </summary>
    public int VoucherType { get; set; }

    /// <summary>
    /// 凭证摘要
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AbstractText { get; set; }

    /// <summary>
    /// 凭证状态（0草稿 1已审核 2已作废）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 借方合计
    /// </summary>
    public decimal TotalDebit { get; set; }

    /// <summary>
    /// 贷方合计
    /// </summary>
    public decimal TotalCredit { get; set; }

    /// <summary>
    /// 制单人ID
    /// </summary>
    public long? PreparedBy { get; set; }

    /// <summary>
    /// 审核人ID
    /// </summary>
    public long? ReviewedBy { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public DateTime? ReviewedTime { get; set; }

    /// <summary>
    /// 凭证分录列表（导航属性，不映射数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<VoucherEntry>? Entries { get; set; }
}
