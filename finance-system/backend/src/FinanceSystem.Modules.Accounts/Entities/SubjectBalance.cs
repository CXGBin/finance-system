using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 科目期初余额实体
/// </summary>
[SugarTable("fm_subject_balance", "科目期初余额表")]
public class SubjectBalance : BaseEntity
{
    /// <summary>
    /// 科目ID
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 会计期间ID
    /// </summary>
    public long PeriodId { get; set; }

    /// <summary>
    /// 期初借方余额
    /// </summary>
    public decimal BeginDebit { get; set; }

    /// <summary>
    /// 期初贷方余额
    /// </summary>
    public decimal BeginCredit { get; set; }

    /// <summary>
    /// 本期借方发生额
    /// </summary>
    public decimal CurrentDebit { get; set; }

    /// <summary>
    /// 本期贷方发生额
    /// </summary>
    public decimal CurrentCredit { get; set; }

    /// <summary>
    /// 期末借方余额
    /// </summary>
    public decimal EndDebit { get; set; }

    /// <summary>
    /// 期末贷方余额
    /// </summary>
    public decimal EndCredit { get; set; }
}
