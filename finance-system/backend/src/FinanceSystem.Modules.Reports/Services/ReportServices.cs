using FinanceSystem.Core.Common;

using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Reports.DTOs;
using FinanceSystem.Modules.Reports.Entities;
using SqlSugar;
using System.Text.Json;

namespace FinanceSystem.Modules.Reports.Services;

/// <summary>
/// 资产负债表服务实现
/// </summary>
public class BalanceSheetService : IBalanceSheetService
{
    private readonly ISqlSugarClient _db;

    public BalanceSheetService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<BalanceSheetResult> GenerateAsync(ReportQuery query)
    {
        if (string.IsNullOrEmpty(query.Period)) throw new BusinessException("请选择会计期间");

        var period = await ParsePeriodAsync(query.Period);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var yearStartPeriod = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == period.PeriodYear && p.PeriodMonth == 1);

        var allSubjects = await _db.Queryable<AccountSubject>().Where(s => s.IsEnabled == 1).ToListAsync();
        var balances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == period.Id).ToListAsync();
        var yearStartBalances = yearStartPeriod != null
            ? await _db.Queryable<SubjectBalance>().Where(b => b.PeriodId == yearStartPeriod.Id).ToListAsync()
            : new List<SubjectBalance>();

        var result = new BalanceSheetResult { ReportDate = period.EndDate.ToString("yyyy-MM-dd") };

        result.Assets = BuildAssetItems(allSubjects, balances, yearStartBalances, query.ShowZero);
        result.Liabilities = BuildLiabilityItems(allSubjects, balances, yearStartBalances, query.ShowZero);
        result.Equity = BuildEquityItems(allSubjects, balances, yearStartBalances, query.ShowZero);

        result.TotalAssets = result.Assets.Sum(a => a.EndingBalance);
        result.TotalLiabilitiesAndEquity = result.Liabilities.Sum(l => l.EndingBalance)
                                          + result.Equity.Sum(e => e.EndingBalance);

        return result;
    }

    private List<BalanceSheetItem> BuildAssetItems(
        List<AccountSubject> subjects, List<SubjectBalance> balances,
        List<SubjectBalance> yearBalances, bool showZero)
    {
        var items = new List<BalanceSheetItem>
        {
            new() { LineNo = 1, ItemName = "货币资金", SubjectCodes = new() { "1001", "1002", "1012" } },
            new() { LineNo = 2, ItemName = "应收账款", SubjectCodes = new() { "1122" } },
            new() { LineNo = 3, ItemName = "预付账款", SubjectCodes = new() { "1123" } },
            new() { LineNo = 4, ItemName = "存货", SubjectCodes = new() { "1401", "1403", "1411", "5001" } },
            new() { LineNo = 5, ItemName = "固定资产", SubjectCodes = new() { "1601" } },
            new() { LineNo = 6, ItemName = "无形资产", SubjectCodes = new() { "1701" } }
        };

        foreach (var item in items)
        {
            var ids = subjects.Where(s => item.SubjectCodes.Any(c => s.SubjectCode.StartsWith(c))).Select(s => s.Id).ToList();
            item.BeginningBalance = CalcNetBalance(balances, ids);
            item.EndingBalance = item.BeginningBalance;
            if (yearBalances.Any()) item.BeginningBalance = CalcNetBalance(yearBalances, ids);
        }

        return showZero ? items : items.Where(i => Math.Abs(i.EndingBalance) > 0.01m).ToList();
    }

    private List<BalanceSheetItem> BuildLiabilityItems(
        List<AccountSubject> subjects, List<SubjectBalance> balances,
        List<SubjectBalance> yearBalances, bool showZero)
    {
        var items = new List<BalanceSheetItem>
        {
            new() { LineNo = 1, ItemName = "短期借款", SubjectCodes = new() { "2001" } },
            new() { LineNo = 2, ItemName = "应付账款", SubjectCodes = new() { "2202" } },
            new() { LineNo = 3, ItemName = "应付职工薪酬", SubjectCodes = new() { "2211" } },
            new() { LineNo = 4, ItemName = "应交税费", SubjectCodes = new() { "2221" } },
            new() { LineNo = 5, ItemName = "长期借款", SubjectCodes = new() { "2501" } }
        };

        foreach (var item in items)
        {
            var ids = subjects.Where(s => item.SubjectCodes.Any(c => s.SubjectCode.StartsWith(c))).Select(s => s.Id).ToList();
            item.BeginningBalance = CalcCreditBalance(balances, ids);
            item.EndingBalance = item.BeginningBalance;
            if (yearBalances.Any()) item.BeginningBalance = CalcCreditBalance(yearBalances, ids);
        }

        return showZero ? items : items.Where(i => Math.Abs(i.EndingBalance) > 0.01m).ToList();
    }

    private List<BalanceSheetItem> BuildEquityItems(
        List<AccountSubject> subjects, List<SubjectBalance> balances,
        List<SubjectBalance> yearBalances, bool showZero)
    {
        var items = new List<BalanceSheetItem>
        {
            new() { LineNo = 1, ItemName = "实收资本", SubjectCodes = new() { "4001" } },
            new() { LineNo = 2, ItemName = "资本公积", SubjectCodes = new() { "4002" } },
            new() { LineNo = 3, ItemName = "盈余公积", SubjectCodes = new() { "4101" } },
            new() { LineNo = 4, ItemName = "未分配利润", SubjectCodes = new() { "4103", "4104" } }
        };

        foreach (var item in items)
        {
            var ids = subjects.Where(s => item.SubjectCodes.Any(c => s.SubjectCode.StartsWith(c))).Select(s => s.Id).ToList();
            item.BeginningBalance = CalcCreditBalance(balances, ids);
            item.EndingBalance = item.BeginningBalance;
            if (yearBalances.Any()) item.BeginningBalance = CalcCreditBalance(yearBalances, ids);
        }

        return showZero ? items : items.Where(i => Math.Abs(i.EndingBalance) > 0.01m).ToList();
    }

    private decimal CalcNetBalance(List<SubjectBalance> balances, List<long> ids)
        => balances.Where(b => ids.Contains(b.SubjectId)).Sum(b => b.EndDebit - b.EndCredit);

    private decimal CalcCreditBalance(List<SubjectBalance> balances, List<long> ids)
        => balances.Where(b => ids.Contains(b.SubjectId)).Sum(b => b.EndCredit - b.EndDebit);

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}

/// <summary>
/// 利润表服务实现
/// </summary>
public class IncomeStatementService : IIncomeStatementService
{
    private readonly ISqlSugarClient _db;

    public IncomeStatementService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<IncomeStatementResult> GenerateAsync(IncomeStatementQuery query)
    {
        if (string.IsNullOrEmpty(query.Period)) throw new BusinessException("请选择会计期间");

        var period = await ParsePeriodAsync(query.Period);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var subjects = await _db.Queryable<AccountSubject>()
            .Where(s => (s.SubjectType == 4 || s.SubjectType == 5) && s.IsEnabled == 1).ToListAsync();

        var periodIds = new List<long> { period.Id };

        // 累计模式：查询年初到当前期间的所有已审核凭证
        if (query.DataType == "cumulative")
        {
            periodIds = await _db.Queryable<AccountingPeriod>()
                .Where(p => p.PeriodYear == period.PeriodYear && p.PeriodMonth <= period.PeriodMonth)
                .Select(p => p.Id).ToListAsync();
        }

        var entryList = await _db.Queryable<VoucherEntry>()
            .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
            .Where((e, v) => periodIds.Contains(v.PeriodId) && v.Status == 1)
            .Select((e, v) => new { e.SubjectId, e.DebitAmount, e.CreditAmount })
            .ToListAsync();

        decimal CalcAmount(List<string> codes, bool isDebit)
        {
            var ids = subjects.Where(s => codes.Any(c => s.SubjectCode.StartsWith(c))).Select(s => s.Id).ToHashSet();
            return isDebit
                ? entryList.Where(d => ids.Contains(d.SubjectId)).Sum(d => d.DebitAmount)
                : entryList.Where(d => ids.Contains(d.SubjectId)).Sum(d => d.CreditAmount);
        }

        decimal revenue = CalcAmount(new() { "6001", "6051", "6101", "6111" }, false);
        decimal cost = CalcAmount(new() { "6401" }, true);
        decimal taxSurcharge = CalcAmount(new() { "6402", "6403" }, true);
        decimal sellingExpense = CalcAmount(new() { "6601" }, true);
        decimal adminExpense = CalcAmount(new() { "6602" }, true);
        decimal financeExpense = CalcAmount(new() { "6603" }, true);
        decimal operatingProfit = revenue - cost - taxSurcharge - sellingExpense - adminExpense - financeExpense;
        decimal nonOperatingIncome = CalcAmount(new() { "6301" }, false);
        decimal nonOperatingExpense = CalcAmount(new() { "6711" }, true);
        decimal totalProfit = operatingProfit + nonOperatingIncome - nonOperatingExpense;
        decimal incomeTax = CalcAmount(new() { "6801" }, true);
        decimal netProfit = totalProfit - incomeTax;

        var items = new List<IncomeStatementItem>
        {
            new() { LineNo = 1, ItemName = "营业收入", CurrentAmount = revenue },
            new() { LineNo = 2, ItemName = "营业成本", CurrentAmount = cost },
            new() { LineNo = 3, ItemName = "税金及附加", CurrentAmount = taxSurcharge },
            new() { LineNo = 4, ItemName = "销售费用", CurrentAmount = sellingExpense },
            new() { LineNo = 5, ItemName = "管理费用", CurrentAmount = adminExpense },
            new() { LineNo = 6, ItemName = "财务费用", CurrentAmount = financeExpense },
            new() { LineNo = 7, ItemName = "营业利润", CurrentAmount = operatingProfit },
            new() { LineNo = 8, ItemName = "营业外收入", CurrentAmount = nonOperatingIncome },
            new() { LineNo = 9, ItemName = "营业外支出", CurrentAmount = nonOperatingExpense },
            new() { LineNo = 10, ItemName = "利润总额", CurrentAmount = totalProfit },
            new() { LineNo = 11, ItemName = "所得税费用", CurrentAmount = incomeTax },
            new() { LineNo = 12, ItemName = "净利润", CurrentAmount = netProfit }
        };

        if (!query.ShowZero) items = items.Where(i => Math.Abs(i.CurrentAmount) > 0.01m).ToList();

        return new IncomeStatementResult { Period = query.Period, DataType = query.DataType, Items = items };
    }

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}

/// <summary>
/// 现金流量表服务实现（简化版）
/// </summary>
public class CashFlowService : ICashFlowService
{
    private readonly ISqlSugarClient _db;

    public CashFlowService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<CashFlowResult> GenerateAsync(ReportQuery query)
    {
        if (string.IsNullOrEmpty(query.Period)) throw new BusinessException("请选择会计期间");

        var period = await ParsePeriodAsync(query.Period);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var cashSubjectIds = await _db.Queryable<AccountSubject>()
            .Where(s => (s.IsCash == 1 || s.IsBank == 1) && s.IsEnabled == 1)
            .Select(s => s.Id).ToListAsync();

        var entries = await _db.Queryable<VoucherEntry>()
            .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
            .Where((e, v) => v.PeriodId == period.Id && v.Status == 1 && cashSubjectIds.Contains(e.SubjectId))
            .Select((e, v) => new { e.DebitAmount, e.CreditAmount })
            .ToListAsync();

        decimal totalInflow = entries.Sum(e => e.DebitAmount);
        decimal totalOutflow = entries.Sum(e => e.CreditAmount);

        return new CashFlowResult
        {
            Period = query.Period,
            OperatingActivities = new List<CashFlowItem>
            {
                new() { LineNo = 1, ItemName = "经营活动现金流入小计", InflowAmount = totalInflow },
                new() { LineNo = 2, ItemName = "经营活动现金流出小计", OutflowAmount = totalOutflow }
            },
            InvestingActivities = new List<CashFlowItem>(),
            FinancingActivities = new List<CashFlowItem>(),
            NetCashIncrease = totalInflow - totalOutflow
        };
    }

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}

/// <summary>
/// 科目余额表服务实现
/// </summary>
public class SubjectBalanceReportService : ISubjectBalanceReportService
{
    private readonly ISqlSugarClient _db;

    public SubjectBalanceReportService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<object> GetReportAsync(SubjectBalanceReportQuery query)
    {
        if (string.IsNullOrEmpty(query.Period)) throw new BusinessException("请选择会计期间");

        var period = await ParsePeriodAsync(query.Period);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var subjects = await _db.Queryable<AccountSubject>()
            .WhereIF(query.SubjectType.HasValue, s => s.SubjectType == query.SubjectType)
            .WhereIF(query.Level.HasValue, s => s.SubjectLevel <= query.Level)
            .Where(s => s.IsEnabled == 1)
            .OrderBy(s => s.SubjectCode).ToListAsync();

        var balances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == period.Id).ToListAsync();

        var result = subjects.Select(s =>
        {
            var b = balances.FirstOrDefault(x => x.SubjectId == s.Id);
            return new
            {
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Level = s.SubjectLevel,
                BeginDebit = b?.BeginDebit ?? 0,
                BeginCredit = b?.BeginCredit ?? 0,
                CurrentDebit = 0m,
                CurrentCredit = 0m,
                EndDebit = b?.EndDebit ?? 0,
                EndCredit = b?.EndCredit ?? 0,
                YearDebit = 0m,
                YearCredit = 0m
            };
        }).ToList();

        if (!query.ShowNoOccurrence)
            result = result.Where(r => Math.Abs(r.EndDebit - r.EndCredit) > 0.01m
                                       || Math.Abs(r.BeginDebit - r.BeginCredit) > 0.01m).ToList();

        return result;
    }

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}

/// <summary>
/// 自定义报表服务实现
/// </summary>
public class CustomReportService : ICustomReportService
{
    private readonly ISqlSugarClient _db;

    public CustomReportService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<object>> GetTemplatesAsync(int pageIndex = 1, int pageSize = 20)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<ReportTemplate>()
            .OrderBy(t => t.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(pageIndex, pageSize, total);
        return new PageResult<object>(total, list.Cast<object>().ToList());
    }

    /// <inheritdoc/>
    public async Task<long> CreateTemplateAsync(ReportTemplateRequest request, long currentUserId)
    {
        var template = new ReportTemplate
        {
            TemplateName = request.TemplateName,
            Description = request.Description,
            TemplateData = request.TemplateData,
            CreatedBy = currentUserId
        };
        await _db.Insertable(template).ExecuteCommandAsync();
        return template.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateTemplateAsync(long id, ReportTemplateRequest request)
    {
        var template = await _db.Queryable<ReportTemplate>().FirstAsync(t => t.Id == id)
            ?? throw new NotFoundException("模板不存在");

        template.TemplateName = request.TemplateName;
        template.Description = request.Description;
        template.TemplateData = request.TemplateData;
        template.UpdatedTime = DateTime.Now;

        await _db.Updateable(template).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteTemplateAsync(long id)
    {
        await _db.Deleteable<ReportTemplate>().Where(t => t.Id == id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<object> GenerateCustomReportAsync(long templateId, string period)
    {
        var template = await _db.Queryable<ReportTemplate>().FirstAsync(t => t.Id == templateId)
            ?? throw new NotFoundException("模板不存在");

        var periodObj = await ParsePeriodAsync(period);
        if (periodObj == null) throw new NotFoundException("会计期间不存在");

        var rows = JsonSerializer.Deserialize<List<CustomReportRow>>(template.TemplateData) ?? new();
        var balances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == periodObj.Id).ToListAsync();

        var result = rows.Select(row =>
        {
            decimal amount = 0;
            if (row.SubjectIds != null && row.SubjectIds.Count > 0)
            {
                amount = row.CalculateType switch
                {
                    "debit" => balances.Where(b => row.SubjectIds.Contains(b.SubjectId)).Sum(b => b.EndDebit),
                    "credit" => balances.Where(b => row.SubjectIds.Contains(b.SubjectId)).Sum(b => b.EndCredit),
                    _ => balances.Where(b => row.SubjectIds.Contains(b.SubjectId)).Sum(b => b.EndDebit - b.EndCredit)
                };
            }
            return new { row.LineNo, row.ItemName, Amount = amount, row.Indent };
        }).ToList();

        // 支持简单公式计算（如 R1+R2-R3，引用其他行金额进行四则运算）
        if (rows.Any(r => !string.IsNullOrEmpty(r.Formula)))
        {
            var rowDict = result.ToDictionary(r => r.LineNo);
            foreach (var row in rows.Where(r => !string.IsNullOrEmpty(r.Formula) && rowDict.ContainsKey(r.LineNo)))
            {
                var formula = row.Formula!;
                // 替换 R{n} 为对应行的金额值
                var evalExpr = System.Text.RegularExpressions.Regex.Replace(
                    formula, @"R(\d+)",
                    match => rowDict.ContainsKey(int.Parse(match.Groups[1].Value))
                        ? rowDict[int.Parse(match.Groups[1].Value)].Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        : "0");
                // 安全计算简单四则表达式
                try
                {
                    var dt = new System.Data.DataTable();
                    var computed = Convert.ToDecimal(dt.Compute(evalExpr, string.Empty));
                    var resultRow = rowDict[row.LineNo];
                    // 用反射更新Amount字段（匿名类型只读，需重新构建）
                    result = result.Select(r => r.LineNo == row.LineNo
                        ? new { r.LineNo, r.ItemName, Amount = computed, r.Indent }
                        : r).ToList();
                    rowDict[row.LineNo] = new { row.LineNo, row.ItemName, Amount = computed, row.Indent };
                }
                catch
                {
                    // 公式计算失败保持原值，不中断报表生成
                }
            }
        }

        return new { TemplateName = template.TemplateName, Period = period, Rows = result };
    }

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}

/// <summary>
/// 自定义报表行定义
/// </summary>
internal class CustomReportRow
{
    public int LineNo { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public List<long>? SubjectIds { get; set; }
    public string CalculateType { get; set; } = "balance";
    public string? Formula { get; set; }
    public int Indent { get; set; }
}

/// <summary>
/// 报表导出服务实现（占位，待集成文件生成库）
/// </summary>
public class ReportExportService : IReportExportService
{
    /// <inheritdoc/>
    public async Task<string> ExportAsync(ExportQuery query)
    {
        await Task.CompletedTask;
        throw new BusinessException("报表导出功能待集成文件生成库后实现");
    }
}

/// <summary>
/// 多期对比服务实现
/// </summary>
public class CompareService : ICompareService
{
    private readonly ISqlSugarClient _db;

    public CompareService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<CompareResult> CompareAsync(CompareQuery query)
    {
        if (query.Periods.Count < 2 || query.Periods.Count > 4)
            throw new BusinessException("请选择2-4个对比期间");

        if (query.Type != "income-statement")
            throw new BusinessException("当前仅支持利润表的多期对比");

        var result = new CompareResult
        {
            ReportType = query.Type,
            Periods = query.Periods,
            Items = new List<CompareItem>
            {
                new() { LineNo = 1, ItemName = "营业收入" },
                new() { LineNo = 2, ItemName = "营业成本" },
                new() { LineNo = 3, ItemName = "净利润" }
            }
        };

        foreach (var periodStr in query.Periods)
        {
            var period = await ParsePeriodAsync(periodStr);
            if (period == null) continue;

            var entryList = await _db.Queryable<VoucherEntry>()
                .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
                .Where((e, v) => v.PeriodId == period.Id && v.Status == 1)
                .Select((e, v) => new { e.DebitAmount, e.CreditAmount })
                .ToListAsync();

            decimal revenue = entryList.Sum(e => e.CreditAmount);
            decimal cost = entryList.Sum(e => e.DebitAmount);
            decimal profit = revenue - cost;

            result.Items[0].Values[periodStr] = revenue;
            result.Items[1].Values[periodStr] = cost;
            result.Items[2].Values[periodStr] = profit;
        }

        return result;
    }

    private async Task<AccountingPeriod?> ParsePeriodAsync(string periodStr)
    {
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return null;
        return await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == int.Parse(parts[0]) && p.PeriodMonth == int.Parse(parts[1]));
    }
}
