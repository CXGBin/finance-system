using FinanceSystem.Core.Common;

namespace FinanceSystem.Modules.Accounts.DTOs;

/// <summary>
/// 会计科目查询条件
/// </summary>
public class SubjectQuery
{
    /// <summary>
    /// 科目编码模糊搜索
    /// </summary>
    public string? SubjectCode { get; set; }

    /// <summary>
    /// 科目名称模糊搜索
    /// </summary>
    public string? SubjectName { get; set; }

    /// <summary>
    /// 科目类型筛选
    /// </summary>
    public int? SubjectType { get; set; }

    /// <summary>
    /// 是否仅启用
    /// </summary>
    public bool? EnabledOnly { get; set; } = true;
}

/// <summary>
/// 新增/编辑科目请求
/// </summary>
public class SubjectCreateRequest
{
    /// <summary>
    /// 科目编码
    /// </summary>
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 科目名称
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// 科目类型（1资产 2负债 3权益 4收入 5费用）
    /// </summary>
    public int SubjectType { get; set; }

    /// <summary>
    /// 余额方向（1借方 2贷方）
    /// </summary>
    public int BalanceDirection { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 是否现金科目
    /// </summary>
    public int IsCash { get; set; }

    /// <summary>
    /// 是否银行科目
    /// </summary>
    public int IsBank { get; set; }

    /// <summary>
    /// 辅助核算类型
    /// </summary>
    public string? AuxiliaryType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 凭证查询条件
/// </summary>
public class VoucherQuery : PageRequest
{
    /// <summary>
    /// 凭证号搜索
    /// </summary>
    public string? VoucherNo { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// 凭证类型
    /// </summary>
    public int? VoucherType { get; set; }

    /// <summary>
    /// 凭证状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 科目ID筛选
    /// </summary>
    public long? SubjectId { get; set; }

    /// <summary>
    /// 摘要关键词
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 新增/编辑凭证请求
/// </summary>
public class VoucherCreateRequest
{
    /// <summary>
    /// 凭证日期
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 凭证类型（1收款 2付款 3转账）
    /// </summary>
    public int VoucherType { get; set; }

    /// <summary>
    /// 凭证摘要
    /// </summary>
    public string? AbstractText { get; set; }

    /// <summary>
    /// 凭证分录列表
    /// </summary>
    public List<VoucherEntryRequest> Entries { get; set; } = new();
}

/// <summary>
/// 凭证分录请求
/// </summary>
public class VoucherEntryRequest
{
    /// <summary>
    /// 分录摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 科目ID（末级）
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 借方金额
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 贷方金额
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 辅助核算ID
    /// </summary>
    public long? AuxiliaryId { get; set; }

    /// <summary>
    /// 辅助核算类型
    /// </summary>
    public string? AuxiliaryType { get; set; }
}

/// <summary>
/// 期初余额保存请求
/// </summary>
public class SubjectBalanceRequest
{
    /// <summary>
    /// 科目ID
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 会计期间ID
    /// </summary>
    public long PeriodId { get; set; }

    /// <summary>
    /// 期初借方余额
    /// </summary>
    public decimal BeginDebit { get; set; }

    /// <summary>
    /// 期初贷方余额
    /// </summary>
    public decimal BeginCredit { get; set; }
}

/// <summary>
/// 账簿查询条件（总账/明细账/日记账通用）
/// </summary>
public class LedgerQuery : PageRequest
{
    /// <summary>
    /// 科目ID
    /// </summary>
    public long? SubjectId { get; set; }

    /// <summary>
    /// 起始期间（如2026-01）
    /// </summary>
    public string? PeriodStart { get; set; }

    /// <summary>
    /// 截止期间
    /// </summary>
    public string? PeriodEnd { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// 摘要关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否显示余额为零
    /// </summary>
    public bool? ShowZeroBalance { get; set; }
}

/// <summary>
/// 辅助核算项通用查询条件
/// </summary>
public class AuxiliaryQuery : PageRequest
{
    /// <summary>
    /// 编码搜索
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 名称搜索
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 新增/编辑项目辅助核算请求
/// </summary>
public class AuxProjectRequest
{
    /// <summary>
    /// 项目编码
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// 项目负责人
    /// </summary>
    public string? Manager { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}

/// <summary>
/// 新增/编辑客户辅助核算请求
/// </summary>
public class AuxCustomerRequest
{
    /// <summary>
    /// 客户编码
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 税号
    /// </summary>
    public string? TaxNo { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}

/// <summary>
/// 新增/编辑供应商辅助核算请求
/// </summary>
public class AuxSupplierRequest
{
    /// <summary>
    /// 供应商编码
    /// </summary>
    public string SupplierCode { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 税号
    /// </summary>
    public string? TaxNo { get; set; }

    /// <summary>
    /// 开户银行
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    public string? BankAccount { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}
