namespace FinanceSystem.Modules.Reports.DTOs;

/// <summary>
/// 报表查询基础参数
/// </summary>
public class ReportQuery
{
    /// <summary>
    /// 会计期间（年-月），如 2026-06
    /// </summary>
    public string? Period { get; set; }

    /// <summary>
    /// 是否显示金额为零的行
    /// </summary>
    public bool ShowZero { get; set; } = false;
}

/// <summary>
/// 利润表查询参数
/// </summary>
public class IncomeStatementQuery : ReportQuery
{
    /// <summary>
    /// 数据类型：month（本月）/ cumulative（本年累计）
    /// </summary>
    public string DataType { get; set; } = "month";

    /// <summary>
    /// 对比期间（可选，用于同比分析）
    /// </summary>
    public string? ComparePeriod { get; set; }
}

/// <summary>
/// 科目余额表查询参数
/// </summary>
public class SubjectBalanceReportQuery : ReportQuery
{
    /// <summary>
    /// 科目类型筛选（可选）
    /// </summary>
    public int? SubjectType { get; set; }

    /// <summary>
    /// 展示到第几级（可选）
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 是否显示无发生额的科目
    /// </summary>
    public bool ShowNoOccurrence { get; set; } = false;
}

/// <summary>
/// 资产负债表行项目
/// </summary>
public class BalanceSheetItem
{
    /// <summary>
    /// 行次
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 年初余额
    /// </summary>
    public decimal BeginningBalance { get; set; }

    /// <summary>
    /// 期末余额
    /// </summary>
    public decimal EndingBalance { get; set; }

    /// <summary>
    /// 对应科目编码列表
    /// </summary>
    public List<string> SubjectCodes { get; set; } = new();
}

/// <summary>
/// 资产负债表结果
/// </summary>
public class BalanceSheetResult
{
    /// <summary>
    /// 报表日期
    /// </summary>
    public string ReportDate { get; set; } = string.Empty;

    /// <summary>
    /// 货币单位
    /// </summary>
    public string Currency { get; set; } = "CNY";

    /// <summary>
    /// 金额单位
    /// </summary>
    public string Unit { get; set; } = "元";

    /// <summary>
    /// 资产项目列表
    /// </summary>
    public List<BalanceSheetItem> Assets { get; set; } = new();

    /// <summary>
    /// 负债项目列表
    /// </summary>
    public List<BalanceSheetItem> Liabilities { get; set; } = new();

    /// <summary>
    /// 所有者权益项目列表
    /// </summary>
    public List<BalanceSheetItem> Equity { get; set; } = new();

    /// <summary>
    /// 资产合计
    /// </summary>
    public decimal TotalAssets { get; set; }

    /// <summary>
    /// 负债和所有者权益合计
    /// </summary>
    public decimal TotalLiabilitiesAndEquity { get; set; }

    /// <summary>
    /// 是否平衡
    /// </summary>
    public bool IsBalanced => Math.Abs(TotalAssets - TotalLiabilitiesAndEquity) < 0.01m;
}

/// <summary>
/// 利润表行项目
/// </summary>
public class IncomeStatementItem
{
    /// <summary>
    /// 行次
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 本期金额
    /// </summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>
    /// 上期金额（用于对比）
    /// </summary>
    public decimal? PreviousAmount { get; set; }

    /// <summary>
    /// 增长率（百分比）
    /// </summary>
    public decimal? GrowthRate { get; set; }
}

/// <summary>
/// 利润表结果
/// </summary>
public class IncomeStatementResult
{
    /// <summary>
    /// 报表期间
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型（本月/累计）
    /// </summary>
    public string DataType { get; set; } = "month";

    /// <summary>
    /// 行项目列表
    /// </summary>
    public List<IncomeStatementItem> Items { get; set; } = new();
}

/// <summary>
/// 现金流量表行项目
/// </summary>
public class CashFlowItem
{
    /// <summary>
    /// 行次
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 流入金额
    /// </summary>
    public decimal InflowAmount { get; set; }

    /// <summary>
    /// 流出金额
    /// </summary>
    public decimal OutflowAmount { get; set; }

    /// <summary>
    /// 净额
    /// </summary>
    public decimal NetAmount => InflowAmount - OutflowAmount;
}

/// <summary>
/// 现金流量表结果
/// </summary>
public class CashFlowResult
{
    /// <summary>
    /// 报表期间
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 经营活动现金流
    /// </summary>
    public List<CashFlowItem> OperatingActivities { get; set; } = new();

    /// <summary>
    /// 投资活动现金流
    /// </summary>
    public List<CashFlowItem> InvestingActivities { get; set; } = new();

    /// <summary>
    /// 筹资活动现金流
    /// </summary>
    public List<CashFlowItem> FinancingActivities { get; set; } = new();

    /// <summary>
    /// 现金净增加额
    /// </summary>
    public decimal NetCashIncrease { get; set; }
}

/// <summary>
/// 自定义报表模板创建/修改请求
/// </summary>
public class ReportTemplateRequest
{
    /// <summary>
    /// 报表名称
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 模板JSON数据
    /// </summary>
    public string TemplateData { get; set; } = "[]";
}

/// <summary>
/// 多期对比查询参数
/// </summary>
public class CompareQuery
{
    /// <summary>
    /// 报表类型（income-statement/subject-balance）
    /// </summary>
    public string Type { get; set; } = "income-statement";

    /// <summary>
    /// 对比期间列表（最多4个）
    /// </summary>
    public List<string> Periods { get; set; } = new();

    /// <summary>
    /// 展示模式：parallel（并列）/ growth_rate（增长率）/ difference（差异额）
    /// </summary>
    public string DisplayMode { get; set; } = "parallel";
}

/// <summary>
/// 多期对比结果
/// </summary>
public class CompareResult
{
    /// <summary>
    /// 报表类型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 对比期间列表
    /// </summary>
    public List<string> Periods { get; set; } = new();

    /// <summary>
    /// 行项目数据（含各期间的值）
    /// </summary>
    public List<CompareItem> Items { get; set; } = new();
}

/// <summary>
/// 多期对比行项目
/// </summary>
public class CompareItem
{
    /// <summary>
    /// 行次
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 各期间的值
    /// </summary>
    public Dictionary<string, decimal> Values { get; set; } = new();
}

/// <summary>
/// 报表导出请求参数
/// </summary>
public class ExportQuery
{
    /// <summary>
    /// 报表类型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 会计期间
    /// </summary>
    public string? Period { get; set; }

    /// <summary>
    /// 导出格式：excel / pdf
    /// </summary>
    public string Format { get; set; } = "excel";
}
