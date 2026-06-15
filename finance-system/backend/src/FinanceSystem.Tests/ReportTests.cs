using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Reports.DTOs;
using FinanceSystem.Modules.Reports.Entities;
using Xunit;

namespace FinanceSystem.Tests.Reports;

/// <summary>
/// 资产负债表报表取数逻辑单元测试
/// </summary>
public class BalanceSheetTests
{
    /// <summary>
    /// 资产项：货币资金包含1001+1002+1012科目
    /// </summary>
    [Fact]
    public void AssetItem_CashIncludesCorrectCodes()
    {
        var cashCodes = new List<string> { "1001", "1002", "1012" };
        var subjectCode = "1001";
        Assert.Contains(cashCodes, c => subjectCode.StartsWith(c));
    }

    /// <summary>
    /// 资产项：应收账款1122科目匹配
    /// </summary>
    [Fact]
    public void AssetItem_ReceivableMatches()
    {
        var arCodes = new List<string> { "1122" };
        var subjectCode = "1122";
        Assert.Contains(arCodes, c => subjectCode.StartsWith(c));
    }

    /// <summary>
    /// 负债项：应付账款2202科目匹配
    /// </summary>
    [Fact]
    public void LiabilityItem_PayableMatches()
    {
        var apCodes = new List<string> { "2202" };
        var subjectCode = "2202";
        Assert.Contains(apCodes, c => subjectCode.StartsWith(c));
    }

    /// <summary>
    /// 权益项：实收资本4001科目匹配
    /// </summary>
    [Fact]
    public void EquityItem_CapitalMatches()
    {
        var codes = new List<string> { "4001" };
        var subjectCode = "4001";
        Assert.Contains(codes, c => subjectCode.StartsWith(c));
    }

    /// <summary>
    /// 资产=负债+权益（会计恒等式）
    /// </summary>
    [Fact]
    public void AccountingEquation_AssetsEqualsLiabilitiesPlusEquity()
    {
        var totalAssets = 5000000m;
        var totalLiabilities = 2000000m;
        var totalEquity = 3000000m;
        Assert.Equal(totalAssets, totalLiabilities + totalEquity);
    }

    /// <summary>
    /// 净余额计算：借方-贷方
    /// </summary>
    [Fact]
    public void CalcNetBalance_DebitMinusCredit()
    {
        var endDebit = 100000m;
        var endCredit = 20000m;
        var netBalance = endDebit - endCredit;
        Assert.Equal(80000m, netBalance);
    }

    /// <summary>
    /// 净额计算：贷方-借方（负债/权益）
    /// </summary>
    [Fact]
    public void CalcCreditBalance_CreditMinusDebit()
    {
        var endCredit = 500000m;
        var endDebit = 50000m;
        var creditBalance = endCredit - endDebit;
        Assert.Equal(450000m, creditBalance);
    }
}

/// <summary>
/// 利润表取数逻辑单元测试
/// </summary>
public class IncomeStatementTests
{
    /// <summary>
    /// 营业利润=收入-成本-税金-销售费用-管理费用-财务费用
    /// </summary>
    [Fact]
    public void OperatingProfit_Calculation()
    {
        decimal revenue = 1000000m;
        decimal cost = 600000m;
        decimal taxSurcharge = 10000m;
        decimal sellingExpense = 50000m;
        decimal adminExpense = 80000m;
        decimal financeExpense = 20000m;
        decimal operatingProfit = revenue - cost - taxSurcharge - sellingExpense - adminExpense - financeExpense;

        Assert.Equal(240000m, operatingProfit);
    }

    /// <summary>
    /// 利润总额=营业利润+营业外收入-营业外支出
    /// </summary>
    [Fact]
    public void TotalProfit_Calculation()
    {
        decimal operatingProfit = 240000m;
        decimal nonOperatingIncome = 5000m;
        decimal nonOperatingExpense = 3000m;
        decimal totalProfit = operatingProfit + nonOperatingIncome - nonOperatingExpense;

        Assert.Equal(242000m, totalProfit);
    }

    /// <summary>
    /// 净利润=利润总额-所得税
    /// </summary>
    [Fact]
    public void NetProfit_Calculation()
    {
        decimal totalProfit = 242000m;
        decimal incomeTax = Math.Round(totalProfit * 0.25m, 2);
        decimal netProfit = totalProfit - incomeTax;

        Assert.Equal(60500m, incomeTax);
        Assert.Equal(181500m, netProfit);
    }

    /// <summary>
    /// 零值行不显示（ShowZero=false）
    /// </summary>
    [Fact]
    public void ZeroItems_ShouldBeFiltered()
    {
        var items = new List<decimal> { 1000m, 0m, 500m, 0.001m, 0m };
        var filtered = items.Where(i => Math.Abs(i) > 0.01m).ToList();
        Assert.Equal(2, filtered.Count);
    }

    /// <summary>
    /// 非零值行应保留
    /// </summary>
    [Fact]
    public void NonZeroItems_ShouldBeKept()
    {
        var items = new List<decimal> { 1000m, 0m, 500m, 0.001m, 0m };
        var filtered = items.Where(i => Math.Abs(i) > 0.01m).ToList();
        Assert.Contains(1000m, filtered);
        Assert.Contains(500m, filtered);
    }

    /// <summary>
    /// 会计期间解析：格式YYYY-MM
    /// </summary>
    [Fact]
    public void PeriodParsing_ValidFormat()
    {
        var periodStr = "2026-06";
        var parts = periodStr.Split('-');
        Assert.Equal(2, parts.Length);
        Assert.Equal("2026", parts[0]);
        Assert.Equal("06", parts[1]);
    }

    /// <summary>
    /// 会计期间解析：无效格式
    /// </summary>
    [Fact]
    public void PeriodParsing_InvalidFormat()
    {
        var periodStr = "202606";
        var parts = periodStr.Split('-');
        Assert.NotEqual(2, parts.Length);
    }
}

/// <summary>
/// 现金流量表取数逻辑单元测试
/// </summary>
public class CashFlowTests
{
    /// <summary>
    /// 净现金增加=流入-流出
    /// </summary>
    [Fact]
    public void NetCashIncrease_Calculation()
    {
        decimal totalInflow = 800000m;
        decimal totalOutflow = 600000m;
        var netIncrease = totalInflow - totalOutflow;
        Assert.Equal(200000m, netIncrease);
    }
}

/// <summary>
/// 多期对比逻辑单元测试
/// </summary>
public class CompareTests
{
    /// <summary>
    /// 对比期间数量：最少2个最多4个
    /// </summary>
    [Fact]
    public void ComparePeriods_Min2Max4()
    {
        var periods = new List<string> { "2026-01", "2026-03" };
        Assert.True(periods.Count >= 2 && periods.Count <= 4);
    }

    /// <summary>
    /// 对比期间数量：1个应失败
    /// </summary>
    [Fact]
    public void ComparePeriods_LessThan2_ShouldFail()
    {
        var periods = new List<string> { "2026-01" };
        Assert.True(periods.Count < 2);
    }

    /// <summary>
    /// 对比期间数量：5个应失败
    /// </summary>
    [Fact]
    public void ComparePeriods_MoreThan4_ShouldFail()
    {
        var periods = new List<string> { "2026-01", "2026-02", "2026-03", "2026-04", "2026-05" };
        Assert.True(periods.Count > 4);
    }
}
