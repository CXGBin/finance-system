using FinanceSystem.Core.Common;
namespace FinanceSystem.Modules.Expense.DTOs;

/// <summary>费用类型请求</summary>
public class ExpenseTypeRequest
{
    /// <summary>
    /// 类型编码
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;
    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;
    public long? SubjectId { get; set; }
    public decimal? SingleLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>报销单查询</summary>
public class ExpenseClaimQuery : PageRequest
{
    public int? Status { get; set; }
    public long? ClaimantId { get; set; }
    public long? DeptId { get; set; }
}

/// <summary>报销单创建/修改</summary>
public class ExpenseClaimRequest
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 报销费用明细列表
    /// </summary>
    public List<ExpenseItemRequest> Items { get; set; } = new();
    public string? Remark { get; set; }
}

/// <summary>报销明细行</summary>
public class ExpenseItemRequest
{
    /// <summary>
    /// 费用类型ID
    /// </summary>
    public long ExpenseTypeId { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// 费用发生日期
    /// </summary>
    public DateTime ExpenseDate { get; set; }
    public string? InvoiceNo { get; set; }
}

/// <summary>费用统计查询</summary>
public class ExpenseStatisticsQuery
{
    /// <summary>
    /// 年份
    /// </summary>
    public int? Year { get; set; }
    /// <summary>
    /// 开始日期
    /// </summary>
    public string StartDate { get; set; } = string.Empty;
    /// <summary>
    /// 结束日期
    /// </summary>
    public string EndDate { get; set; } = string.Empty;
    public long? DeptId { get; set; }
    public long? ExpenseTypeId { get; set; }
}

/// <summary>费用分摊请求</summary>
public class ExpenseAllocateRequest
{
    /// <summary>
    /// 分摊单号
    /// </summary>
    public string AllocateNo { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    public long? DeptId { get; set; }
    /// <summary>
    /// 分摊金额
    /// </summary>
    public decimal AllocateAmount { get; set; }
    /// <summary>
    /// 分摊年度
    /// </summary>
    public int PeriodYear { get; set; }
    /// <summary>
    /// 分摊月份
    /// </summary>
    public int PeriodMonth { get; set; }
}

/// <summary>借款申请请求</summary>
public class ExpenseLoanRequest
{
    public decimal LoanAmount { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
}

/// <summary>借款查询条件</summary>
public class ExpenseLoanQuery
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;
    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 20;
    public int? Status { get; set; }
    public string? Keyword { get; set; }
    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }
    /// <summary>
    /// 排序方向
    /// </summary>
    public string? SortOrder { get; set; }
}
