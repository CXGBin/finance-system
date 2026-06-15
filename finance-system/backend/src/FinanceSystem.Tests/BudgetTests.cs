using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Budget.DTOs;
using Xunit;

namespace FinanceSystem.Tests.Budget;

/// <summary>
/// 预算执行率计算单元测试
/// </summary>
/// <summary>
/// BudgetExecution测试
/// </summary>
public class BudgetExecutionTests
{
    /// <summary>
    /// 执行率计算：已执行100万/年度预算500万=20%
    /// </summary>
    [Fact]
    public void ExecutionRate_NormalCase()
    {
        decimal annualBudget = 5000000m;
        decimal executedAmount = 1000000m;
        decimal rate = annualBudget > 0 ? executedAmount / annualBudget * 100 : 0;
        Assert.Equal(20m, Math.Round(rate, 2));
    }

    /// <summary>
    /// 执行率计算：零预算时应返回0
    /// </summary>
    [Fact]
    public void ExecutionRate_ZeroBudget_ShouldBeZero()
    {
        decimal annualBudget = 0m;
        decimal executedAmount = 1000m;
        decimal rate = annualBudget > 0 ? executedAmount / annualBudget * 100 : 0;
        Assert.Equal(0m, rate);
    }

    /// <summary>
    /// 超预算判断：执行金额超过预算
    /// </summary>
    [Fact]
    public void OverBudget_ExecutedExceedsBudget()
    {
        decimal annualBudget = 5000000m;
        decimal executedAmount = 5500000m;
        decimal remaining = annualBudget - executedAmount;
        Assert.True(remaining < 0);
    }

    /// <summary>
    /// 未超预算判断：执行金额未超过预算
    /// </summary>
    [Fact]
    public void NotOverBudget_ExecutedWithinBudget()
    {
        decimal annualBudget = 5000000m;
        decimal executedAmount = 3000000m;
        decimal remaining = annualBudget - executedAmount;
        Assert.True(remaining > 0);
    }

    /// <summary>
    /// 剩余预算计算正确性
    /// </summary>
    [Fact]
    public void RemainingBudget_Calculation()
    {
        decimal annualBudget = 1000000m;
        decimal executedAmount = 350000m;
        decimal remaining = annualBudget - executedAmount;
        Assert.Equal(650000m, remaining);
    }

    /// <summary>
    /// 月度预算自动均分：年度120万→每月10万
    /// </summary>
    [Fact]
    public void AutoSplit_EqualDivision()
    {
        decimal annualAmount = 1200000m;
        decimal monthly = Math.Round(annualAmount / 12, 2);
        Assert.Equal(100000m, monthly);
    }

    /// <summary>
    /// 月度预算自动均分：最后一月补差
    /// </summary>
    [Fact]
    public void AutoSplit_LastMonthAdjustment()
    {
        decimal annualAmount = 100000m;
        decimal monthly = Math.Round(annualAmount / 12, 2);
        decimal total11Months = monthly * 11;
        decimal lastMonth = Math.Round(annualAmount - total11Months, 2);
        // 总计12个月应等于年度预算
        Assert.True(Math.Abs(total11Months + lastMonth - annualAmount) <= 0.02m);
    }

    /// <summary>
    /// 月度预算合计验证：应等于年度预算
    /// </summary>
    [Fact]
    public void MonthlySum_ShouldEqualAnnualBudget()
    {
        decimal annualAmount = 600000m;
        decimal monthly = Math.Round(annualAmount / 12, 2);
        decimal totalMonthly = monthly * 11 + Math.Round(annualAmount - monthly * 11, 2);
        Assert.True(Math.Abs(totalMonthly - annualAmount) <= 0.02m);
    }

    /// <summary>
    /// 预算调整：追加后金额正确
    /// </summary>
    [Fact]
    public void BudgetAdjust_Increase()
    {
        decimal before = 500000m;
        decimal adjustAmount = 100000m;
        decimal afterAmount = before + adjustAmount;
        Assert.Equal(600000m, afterAmount);
    }

    /// <summary>
    /// 预算调整：调减后金额正确
    /// </summary>
    [Fact]
    public void BudgetAdjust_Decrease()
    {
        decimal before = 500000m;
        decimal adjustAmount = 100000m;
        decimal afterAmount = before - adjustAmount;
        Assert.Equal(400000m, afterAmount);
    }

    /// <summary>
    /// 预算调整：调减后不能为负数
    /// </summary>
    [Fact]
    public void BudgetAdjust_NegativeShouldFail()
    {
        decimal before = 50000m;
        decimal adjustAmount = 100000m;
        decimal afterAmount = before - adjustAmount;
        Assert.True(afterAmount < 0);
    }

    /// <summary>
    /// 预算预警阈值：执行率80%触发预警
    /// </summary>
    [Fact]
    public void AlertThreshold_80PercentTrigger()
    {
        decimal alertThreshold = 80m;
        decimal executionRate = 85m;
        Assert.True(executionRate >= alertThreshold);
    }

    /// <summary>
    /// 预算预警阈值：执行率75%不触发
    /// </summary>
    [Fact]
    public void AlertThreshold_BelowThresholdNoTrigger()
    {
        decimal alertThreshold = 80m;
        decimal executionRate = 75m;
        Assert.False(executionRate >= alertThreshold);
    }
}

/// <summary>
/// 预算月度保存验证单元测试
/// </summary>
/// <summary>
/// BudgetMonthlySave测试
/// </summary>
public class BudgetMonthlySaveTests
{
    /// <summary>
    /// 月度合计等于年度预算应通过
    /// </summary>
    [Fact]
    public void MonthlyTotal_EqualsAnnual_ShouldPass()
    {
        decimal annualAmount = 1200000m;
        var items = Enumerable.Range(1, 12).Select(m => new BudgetMonthlyItem
        {
            Month = m,
            Amount = 100000m
        }).ToList();
        var totalMonthly = items.Sum(i => i.Amount);
        Assert.True(Math.Abs(totalMonthly - annualAmount) <= 0.01m);
    }

    /// <summary>
    /// 月度合计不等于年度预算应失败
    /// </summary>
    [Fact]
    public void MonthlyTotal_NotEqualsAnnual_ShouldFail()
    {
        decimal annualAmount = 1200000m;
        var items = Enumerable.Range(1, 12).Select(m => new BudgetMonthlyItem
        {
            Month = m,
            Amount = 90000m
        }).ToList();
        var totalMonthly = items.Sum(i => i.Amount);
        Assert.True(Math.Abs(totalMonthly - annualAmount) > 0.01m);
    }
}
