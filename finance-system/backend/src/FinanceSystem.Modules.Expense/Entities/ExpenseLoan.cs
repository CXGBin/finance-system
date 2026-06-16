using FinanceSystem.Core.Common;
using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Expense.Entities;

/// <summary>
/// 借款申请实体
/// </summary>
/// <summary>
/// ExpenseLoan
/// </summary>
[SugarTable("fm_expense_loan", "借款申请表")]
public class ExpenseLoan : FullEntity
{
    /// <summary>借款编号</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "借款编号")]
    /// <summary>
    /// 借款编号
    /// </summary>
    public string LoanNo { get; set; } = string.Empty;

    /// <summary>借款人ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "借款人ID")]
    /// <summary>
    /// 借款人ID
    /// </summary>
    public long ApplicantId { get; set; }

    /// <summary>借款金额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "借款金额")]
    /// <summary>
    /// 借款金额
    /// </summary>
    public decimal LoanAmount { get; set; }

    /// <summary>已核销金额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "已核销金额")]
    /// <summary>
    /// 已核销金额
    /// </summary>
    public decimal SettledAmount { get; set; }

    /// <summary>借款事由</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "借款事由")]
    /// <summary>
    /// 借款事由
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>预计还款日期</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "预计还款日期")]
    /// <summary>
    /// 预计还款日期
    /// </summary>
    public DateTime? ExpectedReturnDate { get; set; }

    /// <summary>关联凭证ID（付款凭证）</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "关联凭证ID")]
    /// <summary>
    /// 关联凭证ID
    /// </summary>
    public long? VoucherId { get; set; }

    /// <summary>状态：0=待审批 1=已借出 2=已核销 3=已退回</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "状态")]
    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>审批实例ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "审批实例ID")]
    /// <summary>
    /// 审批实例ID
    /// </summary>
    public long? ApprovalInstanceId { get; set; }
}
