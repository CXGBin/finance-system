using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Tax.Entities;

/// <summary>
/// 税种配置
/// </summary>
[SugarTable("fm_tax_category", "税种配置表")]
/// <summary>
/// TaxCategory
/// </summary>
public class TaxCategory : FullEntity
{
    /// <summary>税种编码</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "税种编码")]
    /// <summary>
    /// 税种编码
    /// </summary>
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>税种名称</summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "税种名称")]
    /// <summary>
    /// 税种名称
    /// </summary>
    public string TaxName { get; set; } = string.Empty;

    /// <summary>默认税率(%)</summary>
    [SugarColumn(DecimalDigits = 4, IsNullable = false, ColumnDescription = "默认税率")]
    /// <summary>
    /// 默认税率
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>计税方式：1从价 2从量 3复合</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "计税方式")]
    /// <summary>
    /// 计税方式
    /// </summary>
    public int CalculationMethod { get; set; } = 1;

    /// <summary>申报周期：1月 2季 3年</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "申报周期")]
    /// <summary>
    /// 申报周期
    /// </summary>
    public int DeclareCycle { get; set; } = 1;

    /// <summary>关联会计科目ID（应交税费-XX）</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "关联科目ID")]
    /// <summary>
    /// 关联科目ID
    /// </summary>
    public long? SubjectId { get; set; }

    /// <summary>是否启用</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 纳税申报记录
/// </summary>
[SugarTable("fm_tax_declaration", "纳税申报记录表")]
/// <summary>
/// TaxDeclaration
/// </summary>
public class TaxDeclaration : FullEntity
{
    /// <summary>税种ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "税种ID")]
    /// <summary>
    /// 税种ID
    /// </summary>
    public long TaxCategoryId { get; set; }

    /// <summary>申报期间</summary>
    [SugarColumn(Length = 10, IsNullable = false, ColumnDescription = "申报期间")]
    /// <summary>
    /// 申报期间
    /// </summary>
    public string DeclarePeriod { get; set; } = string.Empty;

    /// <summary>应纳税额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "应纳税额")]
    /// <summary>
    /// 应纳税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>实际缴税额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "实际缴税额")]
    /// <summary>
    /// 实际缴税额
    /// </summary>
    public decimal ActualPaidAmount { get; set; }

    /// <summary>申报状态：0待申报 1已申报 2已缴纳</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "申报状态")]
    /// <summary>
    /// 申报状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>申报人ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "申报人ID")]
    /// <summary>
    /// 申报人ID
    /// </summary>
    public long DeclaredBy { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 发票登记
/// </summary>
[SugarTable("fm_tax_invoice", "发票登记表")]
/// <summary>
/// TaxInvoice
/// </summary>
public class TaxInvoice : FullEntity
{
    /// <summary>发票类型：1增值税专用发票 2增值税普通发票 3其他</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "发票类型")]
    /// <summary>
    /// 发票类型
    /// </summary>
    public int InvoiceType { get; set; }

    /// <summary>发票号码</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "发票号码")]
    /// <summary>
    /// 发票号码
    /// </summary>
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>开票日期</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "开票日期")]
    /// <summary>
    /// 开票日期
    /// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>销方/购方名称</summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "对方名称")]
    /// <summary>
    /// 对方名称
    /// </summary>
    public string CounterpartyName { get; set; } = string.Empty;

    /// <summary>税额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "税额")]
    /// <summary>
    /// 税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>不含税金额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "不含税金额")]
    /// <summary>
    /// 不含税金额
    /// </summary>
    public decimal AmountWithoutTax { get; set; }

    /// <summary>价税合计</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "价税合计")]
    /// <summary>
    /// 价税合计
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>方向：1进项 2销项</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "方向")]
    /// <summary>
    /// 方向
    /// </summary>
    public int Direction { get; set; }

    /// <summary>关联凭证ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "关联凭证ID")]
    /// <summary>
    /// 关联凭证ID
    /// </summary>
    public long? VoucherId { get; set; }

    /// <summary>是否已认证</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否已认证")]
    /// <summary>
    /// 是否已认证
    /// </summary>
    public int IsVerified { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
