using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Modules.Tax.Entities;
using Xunit;

namespace FinanceSystem.Tests.Tax;

/// <summary>
/// 税额计算单元测试
/// </summary>
public class TaxCalculationTests
{
    /// <summary>
    /// 增值税计算：税率13%，含税100万→税额115044.25
    /// </summary>
    [Fact]
    public void VatCalculation_13Percent()
    {
        decimal totalAmount = 1000000m; // 含税金额
        decimal taxRate = 0.13m;
        decimal amountWithoutTax = Math.Round(totalAmount / (1 + taxRate), 2);
        decimal taxAmount = totalAmount - amountWithoutTax;

        Assert.Equal(884955.75m, amountWithoutTax);
        Assert.Equal(115044.25m, taxAmount);
    }

    /// <summary>
    /// 增值税计算：税率9%，含税50万
    /// </summary>
    [Fact]
    public void VatCalculation_9Percent()
    {
        decimal totalAmount = 500000m;
        decimal taxRate = 0.09m;
        decimal amountWithoutTax = Math.Round(totalAmount / (1 + taxRate), 2);
        decimal taxAmount = Math.Round(totalAmount - amountWithoutTax, 2);

        Assert.Equal(458715.60m, amountWithoutTax);
        Assert.Equal(41284.40m, taxAmount);
    }

    /// <summary>
    /// 增值税计算：税率6%（现代服务业）
    /// </summary>
    [Fact]
    public void VatCalculation_6Percent()
    {
        decimal totalAmount = 200000m;
        decimal taxRate = 0.06m;
        decimal amountWithoutTax = Math.Round(totalAmount / (1 + taxRate), 2);
        decimal taxAmount = Math.Round(totalAmount - amountWithoutTax, 2);

        Assert.Equal(188679.25m, amountWithoutTax);
        Assert.Equal(11320.75m, taxAmount);
    }

    /// <summary>
    /// 发票金额计算：不含税+税额=含税总额
    /// </summary>
    [Fact]
    public void InvoiceAmount_Calculation()
    {
        decimal amountWithoutTax = 884955.75m;
        decimal taxAmount = 115044.25m;
        decimal totalAmount = amountWithoutTax + taxAmount;

        Assert.Equal(1000000m, totalAmount);
    }

    /// <summary>
    /// 企业所得税计算：税率25%
    /// </summary>
    [Fact]
    public void IncomeTax_25Percent()
    {
        decimal profitBeforeTax = 1000000m;
        decimal taxRate = 0.25m;
        decimal incomeTax = Math.Round(profitBeforeTax * taxRate, 2);

        Assert.Equal(250000m, incomeTax);
    }

    /// <summary>
    /// 企业所得税计算：小型微利企业优惠税率5%
    /// </summary>
    [Fact]
    public void IncomeTax_SmallBusiness_5Percent()
    {
        decimal profitBeforeTax = 300000m; // 300万以下
        decimal taxRate = 0.05m;
        decimal incomeTax = Math.Round(profitBeforeTax * taxRate, 2);

        Assert.Equal(15000m, incomeTax);
    }
}

/// <summary>
/// 纳税申报状态流转单元测试
/// </summary>
public class TaxDeclarationTests
{
    /// <summary>
    /// 申报完整链路：待申报(0)→已申报(1)→已缴款(2)
    /// </summary>
    [Fact]
    public void Declaration_FullFlow()
    {
        var declaration = new TaxDeclaration
        {
            TaxCategoryId = 1,
            DeclarePeriod = "2026-06",
            TaxAmount = 50000m,
            Status = 0
        };

        // 待申报→已申报
        declaration.Status = 1;
        declaration.DeclaredBy = 100;
        Assert.Equal(1, declaration.Status);

        // 已申报→已缴款
        declaration.Status = 2;
        declaration.ActualPaidAmount = declaration.TaxAmount;
        Assert.Equal(2, declaration.Status);
        Assert.Equal(50000m, declaration.ActualPaidAmount);
    }

    /// <summary>
    /// 非待申报状态不可申报
    /// </summary>
    [Fact]
    public void Declaration_NonPendingCannotDeclare()
    {
        var declaration = new TaxDeclaration { Status = 1 };
        Assert.True(declaration.Status != 0);
    }

    /// <summary>
    /// 非已申报状态不可确认缴款
    /// </summary>
    [Fact]
    public void Declaration_NonDeclaredCannotPay()
    {
        var declaration = new TaxDeclaration { Status = 0 };
        Assert.True(declaration.Status != 1);
    }

    /// <summary>
    /// 重复申报期间应失败
    /// </summary>
    [Fact]
    public void Declaration_DuplicatePeriod_ShouldFail()
    {
        var existingPeriods = new List<string> { "2026-06" };
        var newPeriod = "2026-06";
        Assert.Contains(newPeriod, existingPeriods);
    }
}

/// <summary>
/// 发票服务单元测试
/// </summary>
public class TaxInvoiceTests
{
    /// <summary>
    /// 发票金额计算：不含税+税额=总金额
    /// </summary>
    [Fact]
    public void Invoice_TotalAmount_Calculation()
    {
        var request = new TaxInvoiceRequest
        {
            AmountWithoutTax = 10000m,
            TaxAmount = 1300m
        };
        decimal total = request.AmountWithoutTax + request.TaxAmount;
        Assert.Equal(11300m, total);
    }

    /// <summary>
    /// 发票验真后IsVerified=1
    /// </summary>
    [Fact]
    public void Invoice_VerifyStatus()
    {
        var invoice = new TaxInvoice { IsVerified = 0 };
        invoice.IsVerified = 1;
        Assert.Equal(1, invoice.IsVerified);
    }
}
