using FinanceSystem.Core.Common;
namespace FinanceSystem.Modules.Tax.DTOs;

/// <summary>税种创建/修改</summary>
public class TaxCategoryRequest
{
    public string TaxCode { get; set; } = string.Empty;
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public int CalculationMethod { get; set; } = 1;
    public int DeclareCycle { get; set; } = 1;
    public long? SubjectId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>纳税申报查询</summary>
public class TaxDeclarationQuery : PageRequest
{
    public long? TaxCategoryId { get; set; }
    public string? DeclarePeriod { get; set; }
    public int? Status { get; set; }
}

/// <summary>发票登记请求</summary>
public class TaxInvoiceRequest
{
    public int InvoiceType { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CounterpartyName { get; set; } = string.Empty;
    public decimal TaxAmount { get; set; }
    public decimal AmountWithoutTax { get; set; }
    public int Direction { get; set; }
    public string? Remark { get; set; }
}

/// <summary>发票查询</summary>
public class TaxInvoiceQuery : PageRequest
{
    public int? Direction { get; set; }
    public int? InvoiceType { get; set; }
    public string? InvoiceNo { get; set; }
    public int? IsVerified { get; set; }
}

/// <summary>税务申报计算请求</summary>
public class TaxCalculateRequest
{
    public long TaxCategoryId { get; set; }
    public string DeclarePeriod { get; set; } = string.Empty;
    /// <summary>手动指定的应纳税额基数（仅当税种未关联科目时使用）</summary>
    public decimal? TaxBase { get; set; }
}
