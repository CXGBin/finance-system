using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Budget.DTOs;
using FinanceSystem.Modules.Budget.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Budget.Services;

/// <summary>
/// 预算年度服务实现
/// </summary>
public class BudgetYearService : IBudgetYearService
{
    private readonly ISqlSugarClient _db;

    public BudgetYearService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<BudgetYear>> GetListAsync(BudgetYearQuery query)
    {
        return await _db.Queryable<BudgetYear>()
            .WhereIF(query.Year.HasValue, y => y.Year == query.Year)
            .WhereIF(query.Status.HasValue, y => y.Status == query.Status)
            .OrderBy(y => y.Year, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(BudgetYearRequest request, long currentUserId)
    {
        var exists = await _db.Queryable<BudgetYear>().AnyAsync(y => y.Year == request.Year);
        if (exists) throw new BusinessException($"预算年度 {request.Year} 已存在");

        var entity = new BudgetYear
        {
            Year = request.Year,
            Status = 0,
            Description = request.Description,
            CreatedBy = currentUserId
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateStatusAsync(long id, int status)
    {
        var year = await _db.Queryable<BudgetYear>().FirstAsync(y => y.Id == id)
            ?? throw new NotFoundException("预算年度不存在");
        year.Status = status;
        await _db.Updateable(year).UpdateColumns(y => y.Status).ExecuteCommandAsync();
    }
}

/// <summary>
/// 预算科目服务实现
/// </summary>
public class BudgetSubjectService : IBudgetSubjectService
{
    private readonly ISqlSugarClient _db;

    public BudgetSubjectService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<BudgetSubject>> GetListAsync(long yearId, int pageIndex = 1, int pageSize = 20)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<BudgetSubject>()
            .Where(s => s.BudgetYearId == yearId)
            .OrderBy(s => s.SubjectId)
            .ToPageListAsync(pageIndex, pageSize, total);
        return new PageResult<BudgetSubject>(total, list);
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(BudgetSubjectRequest request)
    {
        var year = await _db.Queryable<BudgetYear>().FirstAsync(y => y.Id == request.BudgetYearId);
        if (year == null) throw new NotFoundException("预算年度不存在");

        var entity = new BudgetSubject
        {
            BudgetYearId = request.BudgetYearId,
            SubjectId = request.SubjectId,
            DeptId = request.DeptId,
            AnnualAmount = request.AnnualAmount,
            Remark = request.Remark
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, BudgetSubjectRequest request)
    {
        var entity = await _db.Queryable<BudgetSubject>().FirstAsync(s => s.Id == id)
            ?? throw new NotFoundException("预算科目不存在");
        entity.SubjectId = request.SubjectId;
        entity.DeptId = request.DeptId;
        entity.AnnualAmount = request.AnnualAmount;
        entity.Remark = request.Remark;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        await _db.Deleteable<BudgetSubject>().Where(s => s.Id == id).ExecuteCommandAsync();
    }
}

/// <summary>
/// 月度预算服务实现
/// </summary>
public class BudgetMonthlyService : IBudgetMonthlyService
{
    private readonly ISqlSugarClient _db;

    public BudgetMonthlyService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<BudgetMonthly>> GetBySubjectAsync(long budgetSubjectId)
    {
        return await _db.Queryable<BudgetMonthly>()
            .Where(m => m.BudgetSubjectId == budgetSubjectId)
            .OrderBy(m => m.Month)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task SaveAsync(BudgetMonthlySaveRequest request)
    {
        var subject = await _db.Queryable<BudgetSubject>().FirstAsync(s => s.Id == request.BudgetSubjectId)
            ?? throw new NotFoundException("预算科目不存在");

        // 验证月度合计等于年度预算
        var totalMonthly = request.Items.Sum(i => i.Amount);
        if (Math.Abs(totalMonthly - subject.AnnualAmount) > 0.01m)
            throw new BusinessException($"月度预算合计{totalMonthly}不等于年度预算{subject.AnnualAmount}");

        // 删除已有月度数据再重新插入
        await _db.Deleteable<BudgetMonthly>()
            .Where(m => m.BudgetSubjectId == request.BudgetSubjectId)
            .ExecuteCommandAsync();

        var monthlies = request.Items.Select(i => new BudgetMonthly
        {
            BudgetSubjectId = request.BudgetSubjectId,
            Month = i.Month,
            Amount = i.Amount
        }).ToList();

        await _db.Insertable(monthlies).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task AutoSplitAsync(long budgetSubjectId)
    {
        var subject = await _db.Queryable<BudgetSubject>().FirstAsync(s => s.Id == budgetSubjectId)
            ?? throw new NotFoundException("预算科目不存在");

        var monthlyAmount = Math.Round(subject.AnnualAmount / 12, 2);
        var items = Enumerable.Range(1, 12).Select(m => new BudgetMonthly
        {
            BudgetSubjectId = budgetSubjectId,
            Month = m,
            Amount = monthlyAmount
        }).ToList();

        // 最后一月补差
        var diff = Math.Round(subject.AnnualAmount - monthlyAmount * 11, 2);
        items[11].Amount = diff;

        await _db.Deleteable<BudgetMonthly>()
            .Where(m => m.BudgetSubjectId == budgetSubjectId)
            .ExecuteCommandAsync();

        await _db.Insertable(items).ExecuteCommandAsync();
    }
}

/// <summary>
/// 预算执行跟踪服务实现
/// </summary>
public class BudgetExecutionService : IBudgetExecutionService
{
    private readonly ISqlSugarClient _db;

    public BudgetExecutionService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<BudgetExecutionItem>> GetExecutionAsync(BudgetExecutionQuery query)
    {
        var subjects = await _db.Queryable<BudgetSubject>()
            .Where(s => s.BudgetYearId == query.BudgetYearId)
            .WhereIF(query.DeptId.HasValue, s => s.DeptId == query.DeptId)
            .WhereIF(query.SubjectId.HasValue, s => s.SubjectId == query.SubjectId)
            .ToListAsync();

        if (!subjects.Any()) return new List<BudgetExecutionItem>();

        var accountSubjects = await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountSubject>()
            .Where(s => subjects.Select(x => x.SubjectId).Contains(s.Id))
            .ToListAsync();

        // 获取预算年度对应的会计期间
        var periods = await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountingPeriod>()
            .Where(p => subjects.Select(x => x.SubjectId).Any())
            .ToListAsync();

        var yearPeriod = await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == query.BudgetYearId);

        // 简化：从凭证分录获取实际发生额
        var periodIds = new List<long>();
        if (yearPeriod != null)
        {
            periodIds = (await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountingPeriod>()
                .Where(p => p.PeriodYear == yearPeriod.PeriodYear && p.IsClosed == 0)
                .Select(p => p.Id).ToListAsync());
        }

        var result = new List<BudgetExecutionItem>();
        foreach (var bs in subjects)
        {
            var acc = accountSubjects.FirstOrDefault(a => a.Id == bs.SubjectId);
            var executedAmount = 0m;
            if (periodIds.Count > 0)
            {
                executedAmount = (await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.VoucherEntry>()
                    .LeftJoin<FinanceSystem.Modules.Accounts.Entities.Voucher>((e, v) => e.VoucherId == v.Id)
                    .Where((e, v) => periodIds.Contains(v.PeriodId) && e.SubjectId == bs.SubjectId && v.Status == 1)
                    .Select((e, v) => new { e.DebitAmount, e.CreditAmount })
                    .ToListAsync()).Sum(x => x.DebitAmount);
            }

            var rate = bs.AnnualAmount > 0 ? executedAmount / bs.AnnualAmount * 100 : 0;

            result.Add(new BudgetExecutionItem
            {
                BudgetSubjectId = bs.Id,
                SubjectCode = acc?.SubjectCode ?? "",
                SubjectName = acc?.SubjectName ?? "",
                AnnualBudget = bs.AnnualAmount,
                ExecutedAmount = executedAmount,
                ExecutionRate = Math.Round(rate, 2),
                RemainingBudget = bs.AnnualAmount - executedAmount
            });
        }

        return result;
    }
}

/// <summary>
/// 预算调整服务实现
/// </summary>
public class BudgetAdjustService : IBudgetAdjustService
{
    private readonly ISqlSugarClient _db;

    public BudgetAdjustService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<long> CreateAdjustAsync(BudgetAdjustRequest request, long currentUserId)
    {
        var subject = await _db.Queryable<BudgetSubject>().FirstAsync(s => s.Id == request.BudgetSubjectId)
            ?? throw new NotFoundException("预算科目不存在");

        decimal afterAmount = request.AdjustType == 1
            ? subject.AnnualAmount + request.AdjustAmount
            : subject.AnnualAmount - request.AdjustAmount;

        if (afterAmount < 0) throw new BusinessException("调整后金额不能为负数");

        var entity = new BudgetAdjustment
        {
            BudgetSubjectId = request.BudgetSubjectId,
            AdjustType = request.AdjustType,
            BeforeAmount = subject.AnnualAmount,
            AfterAmount = afterAmount,
            Reason = request.Reason,
            ApproveStatus = 0,
            ApplyBy = currentUserId
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task ApproveAdjustAsync(long id, int action, long currentUserId)
    {
        var adj = await _db.Queryable<BudgetAdjustment>().FirstAsync(a => a.Id == id)
            ?? throw new NotFoundException("调整记录不存在");

        if (adj.ApproveStatus != 0) throw new BusinessException("该调整记录已审批");

        adj.ApproveStatus = action; // 1通过 2驳回
        if (action == 1)
        {
            var subject = await _db.Queryable<BudgetSubject>().FirstAsync(s => s.Id == adj.BudgetSubjectId)
                ?? throw new NotFoundException("预算科目不存在");
            subject.AnnualAmount = adj.AfterAmount;
            await _db.Updateable(subject).UpdateColumns(s => s.AnnualAmount).ExecuteCommandAsync();
        }

        await _db.Updateable(adj).UpdateColumns(a => a.ApproveStatus).ExecuteCommandAsync();
    }
}

/// <summary>
/// 预算预警服务实现
/// </summary>
public class BudgetAlertService : IBudgetAlertService
{
    private readonly ISqlSugarClient _db;

    public BudgetAlertService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<BudgetAlertConfig?> GetConfigAsync(long budgetYearId)
    {
        return await _db.Queryable<BudgetAlertConfig>().FirstAsync(c => c.BudgetYearId == budgetYearId);
    }

    /// <inheritdoc/>
    public async Task SaveConfigAsync(BudgetAlertConfigRequest request)
    {
        var existing = await _db.Queryable<BudgetAlertConfig>()
            .FirstAsync(c => c.BudgetYearId == request.BudgetYearId);

        if (existing != null)
        {
            existing.AlertThreshold = request.AlertThreshold;
            existing.IsEnabled = request.IsEnabled;
            existing.AlertMethod = request.AlertMethod;
            await _db.Updateable(existing).ExecuteCommandAsync();
        }
        else
        {
            await _db.Insertable(new BudgetAlertConfig
            {
                BudgetYearId = request.BudgetYearId,
                AlertThreshold = request.AlertThreshold,
                IsEnabled = request.IsEnabled,
                AlertMethod = request.AlertMethod
            }).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<List<BudgetExecutionItem>> CheckAlertsAsync(long budgetYearId)
    {
        var config = await GetConfigAsync(budgetYearId);
        if (config == null || config.IsEnabled != 1) return new List<BudgetExecutionItem>();

        var executionService = new BudgetExecutionService(_db);
        var all = await executionService.GetExecutionAsync(new BudgetExecutionQuery { BudgetYearId = budgetYearId });

        return all.Where(e => e.ExecutionRate >= config.AlertThreshold).ToList();
    }
}
