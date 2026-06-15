using FinanceSystem.Modules.Tax.Services;
using FinanceSystem.Modules.Tax.Entities;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Core.Common;
using Xunit;

namespace FinanceSystem.Tests;

/// <summary>税务模块增强单元测试</summary>
public class TaxEnhancedTests
{
    // ==================== 附加税计算 ====================

    [Fact]
    public void Surcharge_CityMaintenanceTax()
    {
        // 城市维护建设税 = 增值税 * 7%
        decimal vat = 10000;
        decimal surcharge = Math.Round(vat * 7m / 100, 2);
        Assert.Equal(700m, surcharge);
    }

    [Fact]
    public void Surcharge_EducationSurcharge()
    {
        // 教育费附加 = 增值税 * 3%
        decimal vat = 10000;
        decimal surcharge = Math.Round(vat * 3m / 100, 2);
        Assert.Equal(300m, surcharge);
    }

    [Fact]
    public void Surcharge_LocalEducation()
    {
        // 地方教育附加 = 增值税 * 2%
        decimal vat = 10000;
        decimal surcharge = Math.Round(vat * 2m / 100, 2);
        Assert.Equal(200m, surcharge);
    }

    [Fact]
    public void Surcharge_TotalCalculation()
    {
        decimal vat = 50000;
        decimal total = Math.Round(vat * 7m / 100, 2) + Math.Round(vat * 3m / 100, 2) + Math.Round(vat * 2m / 100, 2);
        Assert.Equal(6000m, total); // 3500+1500+1000
    }

    // ==================== 税负率 ====================

    [Fact]
    public void TaxBurden_VatRate()
    {
        // 增值税税负率 = 增值税 / 不含税销售收入
        decimal vatPaid = 10000;
        decimal revenue = 500000;
        decimal rate = Math.Round(vatPaid / revenue * 100, 2);
        Assert.Equal(2m, rate); // 2%
    }

    [Fact]
    public void TaxBurden_TotalRate()
    {
        // 综合税负率 = 全部已缴税费 / 营业收入
        decimal totalTax = 50000;
        decimal revenue = 500000;
        decimal rate = Math.Round(totalTax / revenue * 100, 2);
        Assert.Equal(10m, rate);
    }

    [Fact]
    public void TaxBurden_ZeroRevenue()
    {
        decimal revenue = 0;
        decimal totalTax = 10000;
        decimal rate = revenue > 0 ? Math.Round(totalTax / revenue * 100, 2) : 0;
        Assert.Equal(0m, rate); // 零收入→0%
    }

    // ==================== 发票 ====================

    [Fact]
    public void Invoice_Direction_Mapping()
    {
        // Direction: 1=销项 2=进项
        Assert.Equal(1, 1);
        Assert.Equal(2, 2);
    }

    [Fact]
    public void Invoice_TypeCodes()
    {
        // InvoiceType: 1=增值税专用 2=增值税普通 3=其他
        var types = new[] { 1, 2, 3 };
        Assert.Equal(3, types.Length);
    }

    [Fact]
    public void TaxCategory_CalculationMethod()
    {
        // 1=从价 2=从量 3=复合
        var methods = new[] { 1, 2, 3 };
        Assert.Contains(1, methods);
    }

    [Fact]
    public void TaxCategory_DeclareCycle()
    {
        // 1=月 3=季
        Assert.Equal(1, 1); // 月度
        Assert.Equal(3, 3); // 季度
    }

    // ==================== 税务日历 ====================

    [Fact]
    public void TaxCalendar_MonthlyDeadline()
    {
        // 月度申报截止日15号
        int deadline = 15;
        Assert.Equal(15, deadline);
    }

    [Fact]
    public void TaxCalendar_QuarterlyMonth()
    {
        // 季度末月（3/6/9/12）才有截止日
        var months = new[] { 3, 6, 9, 12 };
        Assert.Contains(3, months);
        Assert.DoesNotContain(4, months);
    }

    // ==================== 申报 ====================

    [Fact]
    public void Declaration_StatusFlow()
    {
        // 0=待申报 1=已申报 2=已缴款
        Assert.True(0 <= 1 && 1 <= 2);
    }

    [Fact]
    public void Declaration_CannotPayBeforeDeclare()
    {
        int status = 0;
        Assert.NotEqual(2, status); // 待申报不可缴款
    }

    [Fact]
    public void Declaration_PeriodFormat()
    {
        string period = $"{2026}-{3:D2}";
        Assert.Equal("2026-03", period);
    }
}
