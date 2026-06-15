using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 会计期间实体
/// </summary>
[SugarTable("fm_period", "会计期间表")]
/// <summary>
/// AccountingPeriod
/// </summary>
public class AccountingPeriod : BaseEntity
{
    /// <summary>
    /// 会计年度
    /// </summary>
    public int PeriodYear { get; set; }

    /// <summary>
    /// 会计月份
    /// </summary>
    public int PeriodMonth { get; set; }

    /// <summary>
    /// 期间开始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 期间结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 是否已结账
    /// </summary>
    public int IsClosed { get; set; }

    /// <summary>
    /// 结账时间
    /// </summary>
    public DateTime? ClosedTime { get; set; }

    /// <summary>
    /// 结账人ID
    /// </summary>
    public long? ClosedBy { get; set; }
}
