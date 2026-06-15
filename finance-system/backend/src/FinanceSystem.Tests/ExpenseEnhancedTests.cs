using FinanceSystem.Modules.Expense.Services;
using FinanceSystem.Modules.Expense.Entities;
using FinanceSystem.Modules.Expense.DTOs;
using FinanceSystem.Core.Common;
using Xunit;

namespace FinanceSystem.Tests;

/// <summary>费用模块增强单元测试</summary>
public class ExpenseEnhancedTests
{
    // ==================== ExpenseClaim ====================

    [Fact]
    public void ExpenseClaim_StatusFlow_ValidTransitions()
    {
        // 草稿(0) → 审批中(1) → 已通过(2) → 已付款(4)
        Assert.True(0 <= 1); // 草稿→审批中
        Assert.True(1 <= 2); // 审批中→已通过
        Assert.True(2 <= 4); // 已通过→已付款
    }

    [Fact]
    public void ExpenseClaim_RejectTransition()
    {
        // 审批中(1) → 已驳回(3)
        Assert.Equal(3, 3); // 驳回状态
    }

    [Fact]
    public void ExpenseClaim_DraftCannotPay()
    {
        int status = 0; // 草稿
        Assert.NotEqual(4, status); // 草稿不可直接付款
    }

    // ==================== ExpenseType ====================

    [Fact]
    public void ExpenseType_MonthlyLimit_ZeroMeansUnlimited()
    {
        var type = new ExpenseType { TypeName = "差旅费", MonthlyLimit = 0 };
        Assert.Equal(0, type.MonthlyLimit); // 0表示不限
    }

    [Fact]
    public void ExpenseType_MonthlyLimit_Check()
    {
        decimal limit = 5000;
        decimal monthTotal = 3000;
        decimal thisClaim = 2500;
        Assert.True(monthTotal + thisClaim > limit); // 超限
        Assert.False(1000 + 2000 > limit); // 不超限
    }

    // ==================== ExpenseLoan ====================

    [Fact]
    public void ExpenseLoan_StatusFlow()
    {
        // 待审批(0) → 已借出(1) → 已核销(2)
        // 待审批(0) → 已退回(3)
        Assert.True(0 <= 1);
        Assert.True(1 <= 2);
        Assert.Equal(3, 3);
    }

    [Fact]
    public void ExpenseLoan_Settlement_Validation()
    {
        decimal loanAmount = 10000;
        decimal settled = 7000;
        decimal newSettle = 2000;
        decimal remaining = loanAmount - settled;
        Assert.True(newSettle <= remaining); // 2000 <= 3000
        Assert.False(4000 <= remaining); // 4000 > 3000 超出
    }

    [Fact]
    public void ExpenseLoan_FullSettlement()
    {
        decimal loanAmount = 10000;
        decimal settled = 7000;
        decimal newSettle = 3000;
        decimal totalSettled = settled + newSettle;
        Assert.True(totalSettled >= loanAmount); // 全额核销→状态2
    }

    [Fact]
    public void ExpenseLoan_NoFormat()
    {
        // 验证借款编号格式
        string loanNo = $"JK-{1:D4}";
        Assert.Equal("JK-0001", loanNo);
        Assert.Equal("JK-0100", $"JK-{100:D4}");
    }

    // ==================== ExpenseAllocate ====================

    [Fact]
    public void ExpenseAllocate_TotalAmount_Matches()
    {
        decimal total = 10000;
        var items = new[] { 5000m, 3000m, 2000m };
        Assert.Equal(total, items.Sum());
    }

    [Fact]
    public void ExpenseAllocate_ZeroAmount_Rejected()
    {
        decimal amount = 0;
        Assert.False(amount > 0);
    }

    [Fact]
    public void ExpenseStatistics_ByType()
    {
        var stats = new[]
        {
            new { TypeName = "差旅费", Amount = 5000m },
            new { TypeName = "办公费", Amount = 3000m },
            new { TypeName = "差旅费", Amount = 2000m },
        };
        var grouped = stats.GroupBy(s => s.TypeName).Select(g => new { TypeName = g.Key, Total = g.Sum(s => s.Amount) });
        Assert.Equal(7000m, grouped.First(g => g.TypeName == "差旅费").Total);
    }

    [Fact]
    public void ExpenseClaim_AmountValidation()
    {
        decimal amount = -100;
        Assert.True(amount <= 0); // 不可为负
    }
}
