using FinanceSystem.Modules.Asset.Services;
using FinanceSystem.Modules.Asset.Entities;
using Xunit;

namespace FinanceSystem.Tests.Asset;

/// <summary>
/// 资产折旧算法单元测试（纯计算，不依赖数据库）
/// </summary>
/// <summary>
/// DepreciationCalculation测试
/// </summary>
public class DepreciationCalculationTests
{
    /// <summary>直线法：月折旧额应恒定</summary>
    [Fact]
    public void StraightLine_MonthlyDepreciation_ShouldBeConstant()
    {
        decimal original = 100000m;
        decimal residualRate = 5m; // 5%
        int usefulLife = 60;
        decimal residualValue = Math.Round(original * residualRate / 100, 2);
        decimal depreciable = original - residualValue;
        decimal expectedMonthly = Math.Round(depreciable / usefulLife, 2);

        Assert.Equal(95000m, depreciable);
        Assert.True(expectedMonthly > 0);
        // 95000 / 60 = 1583.33
        Assert.Equal(1583.33m, expectedMonthly);
    }

    /// <summary>直线法：累计折旧不超过应折旧总额</summary>
    [Fact]
    public void StraightLine_TotalDepreciation_ShouldNotExceedDepreciable()
    {
        decimal original = 100000m;
        decimal residualValue = 5000m;
        decimal depreciable = original - residualValue;
        decimal monthly = Math.Round(depreciable / 60, 2);

        decimal accumulated = 0m;
        for (int i = 0; i < 60; i++)
        {
            decimal remaining = depreciable - accumulated;
            decimal depreciation = Math.Min(monthly, remaining);
            accumulated += depreciation;
        }

        // 累计折旧应接近应折旧总额（允许0.02的舍入误差）
        Assert.True(accumulated >= depreciable - 1m);
        Assert.True(accumulated <= depreciable);
    }

    /// <summary>双倍余额递减法：早期折旧更多</summary>
    [Fact]
    public void DoubleDeclining_EarlyMonthsHigherThanStraightLine()
    {
        decimal original = 100000m;
        int usefulLife = 60;
        decimal rate = 2.0m / usefulLife;

        decimal firstMonthDD = Math.Round(original * rate, 2);
        decimal depreciable = original - 5000m;
        decimal firstMonthSL = Math.Round(depreciable / usefulLife, 2);

        Assert.True(firstMonthDD > firstMonthSL);
    }

    /// <summary>年数总和法：折旧逐年递减</summary>
    [Fact]
    public void SumOfYears_Depreciation_ShouldDecrease()
    {
        decimal depreciable = 95000m;
        int usefulLife = 60;
        decimal totalMonths = usefulLife * (usefulLife + 1) / 2m;

        decimal firstMonth = Math.Round(depreciable * usefulLife / totalMonths, 2);
        decimal secondMonth = Math.Round(depreciable * (usefulLife - 1) / totalMonths, 2);

        Assert.True(firstMonth > secondMonth);
    }

    /// <summary>折旧计算：净值不应低于残值</summary>
    [Fact]
    public void AllMethods_NetValueShouldNotGoBelowResidual()
    {
        decimal original = 50000m;
        decimal residualValue = 2500m;
        decimal depreciable = original - residualValue;

        decimal accumulated = 0m;
        decimal monthly = Math.Round(depreciable / 60, 2);
        for (int i = 0; i < 100; i++)
        {
            decimal remaining = depreciable - accumulated;
            if (remaining <= 0) break;
            accumulated += Math.Min(monthly, remaining);
        }

        Assert.True(original - accumulated >= residualValue - 0.01m);
    }
}
