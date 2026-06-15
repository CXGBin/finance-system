using FinanceSystem.Core.Common;
namespace FinanceSystem.Modules.Expense.DTOs;

/// <summary>费用类型请求</summary>
public class ExpenseTypeRequest
{
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public long? SubjectId { get; set; }
    public decimal? SingleLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
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
    public string Title { get; set; } = string.Empty;
    public List<ExpenseItemRequest> Items { get; set; } = new();
    public string? Remark { get; set; }
}

/// <summary>报销明细行</summary>
public class ExpenseItemRequest
{
    public long ExpenseTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? InvoiceNo { get; set; }
}

/// <summary>费用统计查询</summary>
public class ExpenseStatisticsQuery
{
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public long? DeptId { get; set; }
    public long? ExpenseTypeId { get; set; }
}

/// <summary>费用分摊请求</summary>
public class ExpenseAllocateRequest
{
    public string AllocateNo { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public long? DeptId { get; set; }
    public decimal AllocateAmount { get; set; }
    public int PeriodYear { get; set; }
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
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? Status { get; set; }
    public string? Keyword { get; set; }
}
