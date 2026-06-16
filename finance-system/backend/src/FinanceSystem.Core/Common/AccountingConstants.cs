namespace FinanceSystem.Core.Common;

/// <summary>
/// 会计科目代码常量集合
/// 用于统一维护各报表和税务计算中引用的科目代码前缀
/// </summary>
public static class AccountingConstants
{
    /// <summary>
    /// 收入类科目代码（主营业务收入、其他业务收入、投资收益、公允价值变动损益等）
    /// </summary>
    public static readonly string[] RevenueSubjectCodes = { "6001", "6051", "6101", "6111" };

    /// <summary>
    /// 成本税金类科目代码（主营业务成本、其他业务成本、税金及附加）
    /// </summary>
    public static readonly string[] CostSubjectCodes = { "6401", "6402", "6403" };

    /// <summary>
    /// 销售费用科目代码
    /// </summary>
    public static readonly string[] SellingExpenseCodes = { "6601" };

    /// <summary>
    /// 管理费用科目代码
    /// </summary>
    public static readonly string[] AdminExpenseCodes = { "6602" };

    /// <summary>
    /// 财务费用科目代码
    /// </summary>
    public static readonly string[] FinanceExpenseCodes = { "6603" };

    /// <summary>
    /// 营业外收入科目代码
    /// </summary>
    public static readonly string[] NonOperatingIncomeCodes = { "6301" };

    /// <summary>
    /// 营业外支出科目代码
    /// </summary>
    public static readonly string[] NonOperatingExpenseCodes = { "6711" };

    /// <summary>
    /// 所得税费用科目代码
    /// </summary>
    public static readonly string[] IncomeTaxCodes = { "6801" };

    /// <summary>
    /// 经营活动相关科目代码（收入/成本/费用等损益类科目）
    /// </summary>
    public static readonly string[] OperatingSubjectCodes =
        { "6001", "6051", "6401", "6402", "6403", "6601", "6602", "6603", "6101", "6111", "6701", "6711" };

    /// <summary>
    /// 投资活动相关科目代码（投资性资产、长期股权投资、固定资产、无形资产等）
    /// </summary>
    public static readonly string[] InvestingSubjectCodes = { "1511", "1601", "1602", "1604", "1701", "1702" };

    /// <summary>
    /// 筹资活动相关科目代码（借款、应付利息、应付股利、实收资本等）
    /// </summary>
    public static readonly string[] FinancingSubjectCodes = { "2001", "2501", "2502", "2231", "2232", "4001" };

    #region 资产负债表科目分组

    /// <summary>
    /// 资产负债表 - 货币资金科目
    /// </summary>
    public static readonly string[] BalanceSheetCashCodes = { "1001", "1002", "1012" };

    /// <summary>
    /// 资产负债表 - 应收账款科目
    /// </summary>
    public static readonly string[] BalanceSheetReceivableCodes = { "1122" };

    /// <summary>
    /// 资产负债表 - 预付账款科目
    /// </summary>
    public static readonly string[] BalanceSheetPrepaymentCodes = { "1123" };

    /// <summary>
    /// 资产负债表 - 存货科目
    /// </summary>
    public static readonly string[] BalanceSheetInventoryCodes = { "1401", "1403", "1411", "5001" };

    /// <summary>
    /// 资产负债表 - 固定资产科目
    /// </summary>
    public static readonly string[] BalanceSheetFixedAssetCodes = { "1601" };

    /// <summary>
    /// 资产负债表 - 无形资产科目
    /// </summary>
    public static readonly string[] BalanceSheetIntangibleAssetCodes = { "1701" };

    /// <summary>
    /// 资产负债表 - 短期借款科目
    /// </summary>
    public static readonly string[] BalanceSheetShortTermLoanCodes = { "2001" };

    /// <summary>
    /// 资产负债表 - 应付账款科目
    /// </summary>
    public static readonly string[] BalanceSheetPayableCodes = { "2202" };

    /// <summary>
    /// 资产负债表 - 应付职工薪酬科目
    /// </summary>
    public static readonly string[] BalanceSheetEmployeePayCodes = { "2211" };

    /// <summary>
    /// 资产负债表 - 应交税费科目
    /// </summary>
    public static readonly string[] BalanceSheetTaxPayableCodes = { "2221" };

    /// <summary>
    /// 资产负债表 - 长期借款科目
    /// </summary>
    public static readonly string[] BalanceSheetLongTermLoanCodes = { "2501" };

    /// <summary>
    /// 资产负债表 - 实收资本科目
    /// </summary>
    public static readonly string[] BalanceSheetPaidInCapitalCodes = { "4001" };

    /// <summary>
    /// 资产负债表 - 资本公积科目
    /// </summary>
    public static readonly string[] BalanceSheetCapitalReserveCodes = { "4002" };

    /// <summary>
    /// 资产负债表 - 盈余公积科目
    /// </summary>
    public static readonly string[] BalanceSheetSurplusReserveCodes = { "4101" };

    /// <summary>
    /// 资产负债表 - 未分配利润科目
    /// </summary>
    public static readonly string[] BalanceSheetRetainedEarningsCodes = { "4103", "4104" };

    #endregion
}
