using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Expense.Entities;

/// <summary>
/// 费用类型
/// </summary>
[SugarTable("fm_expense_type", "费用类型表")]
public class ExpenseType : FullEntity
{
    /// <summary>费用类型编码</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "编码")]
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>费用类型名称</summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "名称")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>关联会计科目ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "关联科目ID")]
    public long? SubjectId { get; set; }

    /// <summary>单次报销限额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "单次限额")]
    public decimal? SingleLimit { get; set; }

    /// <summary>月度累计限额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "月度限额")]
    public decimal? MonthlyLimit { get; set; }

    /// <summary>排序号</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "排序号")]
    public int SortOrder { get; set; }

    /// <summary>是否启用</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public int IsEnabled { get; set; } = 1;
}

/// <summary>
/// 费用报销单
/// </summary>
[SugarTable("fm_expense_claim", "费用报销单表")]
public class ExpenseClaim : FullEntity
{
    /// <summary>报销单号</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "报销单号")]
    public string ClaimNo { get; set; } = string.Empty;

    /// <summary>报销标题</summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "报销标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>报销人ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "报销人ID")]
    public long ClaimantId { get; set; }

    /// <summary>报销人部门ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "部门ID")]
    public long? DeptId { get; set; }

    /// <summary>报销总金额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "总金额")]
    public decimal TotalAmount { get; set; }

    /// <summary>状态：0草稿 1审批中 2已通过 3已驳回 4已付款</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>审批实例ID（关联审批模块）</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "审批实例ID")]
    public long? ApprovalInstanceId { get; set; }

    /// <summary>付款日期</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "付款日期")]
    public DateTime? PaymentDate { get; set; }

    /// <summary>关联凭证ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "凭证ID")]
    public long? VoucherId { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}

/// <summary>
/// 费用报销明细行
/// </summary>
[SugarTable("fm_expense_item", "费用报销明细表")]
public class ExpenseItem : FullEntity
{
    /// <summary>报销单ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "报销单ID")]
    public long ClaimId { get; set; }

    /// <summary>费用类型ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "费用类型ID")]
    public long ExpenseTypeId { get; set; }

    /// <summary>费用说明</summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "费用说明")]
    public string Description { get; set; } = string.Empty;

    /// <summary>费用金额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "费用金额")]
    public decimal Amount { get; set; }

    /// <summary>费用日期</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "费用日期")]
    public DateTime ExpenseDate { get; set; }

    /// <summary>发票号</summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "发票号")]
    public string? InvoiceNo { get; set; }
}
