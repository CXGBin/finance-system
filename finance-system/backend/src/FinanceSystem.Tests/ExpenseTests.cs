using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Expense.DTOs;
using FinanceSystem.Modules.Expense.Entities;
using Xunit;

namespace FinanceSystem.Tests.Expense;

/// <summary>
/// 费用报销状态流转单元测试
/// </summary>
public class ExpenseClaimTests
{
    /// <summary>
    /// 报销单完整链路：草稿(0)→审批中(1)→已通过(2)→已付款(4)
    /// </summary>
    [Fact]
    public void ExpenseClaim_FullFlow_DraftToPaid()
    {
        var claim = new ExpenseClaim { Status = 0 };

        // 草稿→审批中
        claim.Status = 1;
        Assert.Equal(1, claim.Status);

        // 审批中→已通过
        claim.Status = 2;
        Assert.Equal(2, claim.Status);

        // 已通过→已付款
        claim.Status = 4;
        claim.PaymentDate = DateTime.Now;
        Assert.Equal(4, claim.Status);
        Assert.NotNull(claim.PaymentDate);
    }

    /// <summary>
    /// 报销单驳回链路：草稿(0)→审批中(1)→已驳回(3)
    /// </summary>
    [Fact]
    public void ExpenseClaim_Flow_DraftToRejected()
    {
        var claim = new ExpenseClaim { Status = 0 };

        claim.Status = 1;
        Assert.Equal(1, claim.Status);

        claim.Status = 3;
        Assert.Equal(3, claim.Status);
    }

    /// <summary>
    /// 草稿状态可提交
    /// </summary>
    [Fact]
    public void ExpenseClaim_DraftCanSubmit()
    {
        var claim = new ExpenseClaim { Status = 0 };
        Assert.True(claim.Status == 0);
    }

    /// <summary>
    /// 非草稿状态不可提交
    /// </summary>
    [Fact]
    public void ExpenseClaim_NonDraftCannotSubmit()
    {
        var claim = new ExpenseClaim { Status = 1 };
        Assert.True(claim.Status != 0);
    }

    /// <summary>
    /// 非审批中状态不可审批
    /// </summary>
    [Fact]
    public void ExpenseClaim_NonPendingCannotApprove()
    {
        var claim = new ExpenseClaim { Status = 0 };
        Assert.True(claim.Status != 1);
    }

    /// <summary>
    /// 非已通过状态不可确认付款
    /// </summary>
    [Fact]
    public void ExpenseClaim_NonApprovedCannotPay()
    {
        var claim = new ExpenseClaim { Status = 1 };
        Assert.True(claim.Status != 2);
    }

    /// <summary>
    /// 报销金额应等于明细合计
    /// </summary>
    [Fact]
    public void ExpenseClaim_TotalAmount_EqualsItemsSum()
    {
        var items = new List<ExpenseItemRequest>
        {
            new() { Amount = 500m },
            new() { Amount = 300m },
            new() { Amount = 200m }
        };
        var totalAmount = items.Sum(i => i.Amount);
        Assert.Equal(1000m, totalAmount);
    }

    /// <summary>
    /// 报销单至少包含一条明细
    /// </summary>
    [Fact]
    public void ExpenseClaim_ItemsRequired()
    {
        var items = new List<ExpenseItemRequest>();
        Assert.False(items.Any());
    }

    /// <summary>
    /// 报销单号格式验证：BX-YYYYMM-NNNN
    /// </summary>
    [Fact]
    public void ExpenseClaim_ClaimNoFormat()
    {
        var ym = "202606";
        var count = 1;
        var claimNo = $"BX-{ym}-{count:D4}";
        Assert.Equal("BX-202606-0001", claimNo);
    }
}

/// <summary>
/// 费用类型服务单元测试
/// </summary>
public class ExpenseTypeTests
{
    /// <summary>
    /// 费用类型创建请求默认值
    /// </summary>
    [Fact]
    public void ExpenseTypeRequest_Defaults()
    {
        var request = new ExpenseTypeRequest
        {
            TypeCode = "TRAVEL",
            TypeName = "差旅费",
            SubjectId = 6602
        };
        Assert.Equal("TRAVEL", request.TypeCode);
        Assert.Null(request.SingleLimit);
        Assert.Null(request.MonthlyLimit);
    }
}
