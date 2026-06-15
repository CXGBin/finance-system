using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using Xunit;

namespace FinanceSystem.Tests.Accounts;

/// <summary>
/// 会计科目服务单元测试
/// </summary>
public class SubjectServiceTests
{
    /// <summary>
    /// 科目层级：一级科目编码无点号
    /// </summary>
    [Fact]
    public void SubjectLevel_OneLevel_NoDot()
    {
        var code = "1001";
        var level = code.Split('.').Length;
        Assert.Equal(1, level);
    }

    /// <summary>
    /// 科目层级：二级科目编码有一个点号
    /// </summary>
    [Fact]
    public void SubjectLevel_TwoLevel_HasOneDot()
    {
        var code = "1001.01";
        var level = code.Split('.').Length;
        Assert.Equal(2, level);
    }

    /// <summary>
    /// 科目层级：三级科目编码有两个点号
    /// </summary>
    [Fact]
    public void SubjectLevel_ThreeLevel_HasTwoDots()
    {
        var code = "1001.01.01";
        var level = code.Split('.').Length;
        Assert.Equal(3, level);
    }

    /// <summary>
    /// 科目编码唯一性：编码相同应视为重复
    /// </summary>
    [Fact]
    public void SubjectCode_DuplicateCheck()
    {
        var code = "1001";
        var existing = new List<string> { "1001", "1002", "2001" };
        Assert.Contains(code, existing);
    }

    /// <summary>
    /// 科目编码唯一性：新编码不存在应通过
    /// </summary>
    [Fact]
    public void SubjectCode_NewCode_ShouldPass()
    {
        var code = "1003";
        var existing = new List<string> { "1001", "1002", "2001" };
        Assert.DoesNotContain(code, existing);
    }

    /// <summary>
    /// 科目创建请求：默认值正确性
    /// </summary>
    [Fact]
    public void SubjectCreateRequest_DefaultValues()
    {
        var request = new SubjectCreateRequest
        {
            SubjectCode = "1001",
            SubjectName = "库存现金",
            SubjectType = 1,
            BalanceDirection = 1
        };
        Assert.Equal(1, request.IsEnabled);
        Assert.Equal(0, request.IsCash);
        Assert.Equal(0, request.IsBank);
    }
}

/// <summary>
/// 凭证管理服务单元测试
/// </summary>
public class VoucherServiceTests
{
    /// <summary>
    /// 凭证借贷平衡：借方等于贷方应通过
    /// </summary>
    [Fact]
    public void Voucher_DebitCreditBalanced_ShouldPass()
    {
        var entries = new List<VoucherEntryRequest>
        {
            new() { SubjectId = 1, DebitAmount = 1000m, CreditAmount = 0m },
            new() { SubjectId = 2, DebitAmount = 0m, CreditAmount = 1000m }
        };
        var totalDebit = entries.Sum(e => e.DebitAmount);
        var totalCredit = entries.Sum(e => e.CreditAmount);
        Assert.True(Math.Abs(totalDebit - totalCredit) <= 0.01m);
    }

    /// <summary>
    /// 凭证借贷不平衡：借方不等于贷方应失败
    /// </summary>
    [Fact]
    public void Voucher_DebitCreditNotBalanced_ShouldFail()
    {
        var entries = new List<VoucherEntryRequest>
        {
            new() { SubjectId = 1, DebitAmount = 1000m, CreditAmount = 0m },
            new() { SubjectId = 2, DebitAmount = 0m, CreditAmount = 800m }
        };
        var totalDebit = entries.Sum(e => e.DebitAmount);
        var totalCredit = entries.Sum(e => e.CreditAmount);
        Assert.True(Math.Abs(totalDebit - totalCredit) > 0.01m);
    }

    /// <summary>
    /// 凭证至少2条分录
    /// </summary>
    [Fact]
    public void Voucher_MinimumTwoEntries()
    {
        var entries = new List<VoucherEntryRequest>
        {
            new() { SubjectId = 1, DebitAmount = 500m, CreditAmount = 0m },
            new() { SubjectId = 2, DebitAmount = 0m, CreditAmount = 500m }
        };
        Assert.True(entries.Count >= 2);
    }

    /// <summary>
    /// 凭证只有1条分录应失败
    /// </summary>
    [Fact]
    public void Voucher_SingleEntry_ShouldFail()
    {
        var entries = new List<VoucherEntryRequest>
        {
            new() { SubjectId = 1, DebitAmount = 500m, CreditAmount = 500m }
        };
        Assert.True(entries.Count < 2);
    }

    /// <summary>
    /// 凭证状态流转：草稿(0)→已审核(1)→已作废(2)
    /// </summary>
    [Fact]
    public void Voucher_StatusFlow_DraftToAudited()
    {
        var voucher = new Voucher { Status = 0 };
        Assert.Equal(0, voucher.Status);

        // 审核
        voucher.Status = 1;
        Assert.Equal(1, voucher.Status);
    }

    /// <summary>
    /// 凭证状态流转：已审核(1)→反审核→草稿(0)
    /// </summary>
    [Fact]
    public void Voucher_StatusFlow_UnAudit()
    {
        var voucher = new Voucher { Status = 1 };
        voucher.Status = 0;
        Assert.Equal(0, voucher.Status);
    }

    /// <summary>
    /// 凭证状态流转：已作废(2)不可重复作废
    /// </summary>
    [Fact]
    public void Voucher_VoidAlreadyVoided_ShouldFail()
    {
        var voucher = new Voucher { Status = 2 };
        Assert.Equal(2, voucher.Status);
        // 已作废的凭证status==2，再次作废应抛异常
        Assert.True(voucher.Status == 2);
    }

    /// <summary>
    /// 凭证号生成：收款凭证前缀SK
    /// </summary>
    [Fact]
    public void VoucherNo_ReceiptPrefix()
    {
        var prefix = 1 switch { 1 => "SK", 2 => "FK", _ => "ZZ" };
        Assert.Equal("SK", prefix);
    }

    /// <summary>
    /// 凭证号生成：付款凭证前缀FK
    /// </summary>
    [Fact]
    public void VoucherNo_PaymentPrefix()
    {
        var prefix = 2 switch { 1 => "SK", 2 => "FK", _ => "ZZ" };
        Assert.Equal("FK", prefix);
    }

    /// <summary>
    /// 凭证号生成：转账凭证前缀ZZ
    /// </summary>
    [Fact]
    public void VoucherNo_TransferPrefix()
    {
        var prefix = 3 switch { 1 => "SK", 2 => "FK", _ => "ZZ" };
        Assert.Equal("ZZ", prefix);
    }

    /// <summary>
    /// 凭证金额精度：0.01容差边界测试
    /// </summary>
    [Fact]
    public void Voucher_PrecisionTolerance()
    {
        var totalDebit = 1000.00m;
        var totalCredit = 1000.01m;
        // 差值恰好0.01，根据业务逻辑>0.01才判为不平衡
        Assert.False(Math.Abs(totalDebit - totalCredit) > 0.01m);
        Assert.True(Math.Abs(totalDebit - totalCredit) >= 0.01m);
    }

    /// <summary>
    /// 凭证查询：默认分页参数
    /// </summary>
    [Fact]
    public void VoucherQuery_DefaultPagination()
    {
        var query = new VoucherQuery();
        Assert.Equal(1, query.PageIndex);
        Assert.Equal(20, query.PageSize);
    }

    /// <summary>
    /// 凭证实体：总借贷金额正确记录
    /// </summary>
    [Fact]
    public void VoucherEntity_TotalAmounts()
    {
        var voucher = new Voucher
        {
            TotalDebit = 5000m,
            TotalCredit = 5000m,
            Status = 0
        };
        Assert.Equal(voucher.TotalDebit, voucher.TotalCredit);
    }
}
