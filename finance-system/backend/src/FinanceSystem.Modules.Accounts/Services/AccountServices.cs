using FinanceSystem.Core.Common;

using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Accounts.Services;

/// <summary>
/// 会计期间服务实现
/// </summary>
public class PeriodService : IPeriodService
{
    private readonly ISqlSugarClient _db;

    public PeriodService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<AccountingPeriod>> GetListByYearAsync(int year)
    {
        return await _db.Queryable<AccountingPeriod>()
            .Where(p => p.PeriodYear == year)
            .OrderBy(p => p.PeriodMonth)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<AccountingPeriod?> GetCurrentAsync()
    {
        var list = await _db.Queryable<AccountingPeriod>()
            .Where(p => p.IsClosed == 0)
            .OrderBy(p => p.PeriodYear)
            .OrderBy(p => p.PeriodMonth)
            .ToListAsync();
        return list.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task InitYearAsync(int year)
    {
        var exists = await _db.Queryable<AccountingPeriod>().AnyAsync(p => p.PeriodYear == year);
        if (exists) throw new BusinessException($"年度 {year} 的会计期间已存在");

        var periods = new List<AccountingPeriod>();
        for (int month = 1; month <= 12; month++)
        {
            periods.Add(new AccountingPeriod
            {
                PeriodYear = year,
                PeriodMonth = month,
                BeginDate = new DateTime(year, month, 1),
                EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                IsClosed = 0
            });
        }
        await _db.Insertable(periods).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task CloseAsync(long periodId, long currentUserId)
    {
        var period = await _db.Queryable<AccountingPeriod>().FirstAsync(p => p.Id == periodId)
            ?? throw new NotFoundException("会计期间不存在");

        if (period.IsClosed == 1) throw new BusinessException("该期间已结账");

        var hasDraft = await _db.Queryable<Voucher>()
            .AnyAsync(v => v.PeriodId == periodId && v.Status == 0);
        if (hasDraft) throw new BusinessException("当期存在未审核的凭证，请先审核所有凭证");

        period.IsClosed = 1;
        period.ClosedTime = DateTime.Now;
        period.ClosedBy = currentUserId;

        await _db.Updateable(period)
            .UpdateColumns(p => new { p.IsClosed, p.ClosedTime, p.ClosedBy })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task UnCloseAsync(long periodId)
    {
        var period = await _db.Queryable<AccountingPeriod>().FirstAsync(p => p.Id == periodId)
            ?? throw new NotFoundException("会计期间不存在");

        if (period.IsClosed == 0) throw new BusinessException("该期间未结账");

        var nextPeriod = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == period.PeriodYear && p.PeriodMonth == period.PeriodMonth + 1);
        if (nextPeriod != null && nextPeriod.IsClosed == 1)
            throw new BusinessException("下一期间已结账，请先反结账下一期间");

        period.IsClosed = 0;
        period.ClosedTime = null;
        period.ClosedBy = null;

        await _db.Updateable(period)
            .UpdateColumns(p => new { p.IsClosed, p.ClosedTime, p.ClosedBy })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task ProfitTransferAsync(long periodId, long currentUserId)
    {
        var period = await _db.Queryable<AccountingPeriod>().FirstAsync(p => p.Id == periodId)
            ?? throw new NotFoundException("会计期间不存在");

        var hasTransfer = await _db.Queryable<Voucher>()
            .AnyAsync(v => v.PeriodId == periodId && v.AbstractText == "期末损益结转");
        if (hasTransfer) throw new BusinessException("当期已执行损益结转");

        // TODO: 根据科目余额表计算每个损益科目的余额，生成结转凭证
    }
}

/// <summary>
/// 科目期初余额服务实现
/// </summary>
public class SubjectBalanceService : ISubjectBalanceService
{
    private readonly ISqlSugarClient _db;

    public SubjectBalanceService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SubjectBalance>> GetByPeriodAsync(long periodId)
    {
        return await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == periodId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task SaveAsync(List<SubjectBalanceRequest> items)
    {
        var balances = items.Select(item => new SubjectBalance
        {
            SubjectId = item.SubjectId,
            PeriodId = item.PeriodId,
            BeginDebit = item.BeginDebit,
            BeginCredit = item.BeginCredit
        }).ToList();

        await _db.Storageable(balances)
            .WhereColumns(b => new { b.SubjectId, b.PeriodId })
            .ToStorage()
            .AsInsertable.ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<(bool IsBalanced, decimal DebitTotal, decimal CreditTotal)> TrialBalanceAsync(long periodId)
    {
        var balances = await GetByPeriodAsync(periodId);
        var debitTotal = balances.Sum(b => b.BeginDebit);
        var creditTotal = balances.Sum(b => b.BeginCredit);
        return (Math.Abs(debitTotal - creditTotal) < 0.01m, debitTotal, creditTotal);
    }
}

/// <summary>
/// 账簿查询服务实现
/// </summary>
public class LedgerService : ILedgerService
{
    private readonly ISqlSugarClient _db;

    public LedgerService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SubjectBalance>> GetGeneralLedgerAsync(LedgerQuery query)
    {
        var queryable = _db.Queryable<SubjectBalance>();

        if (!string.IsNullOrEmpty(query.PeriodStart))
        {
            var startPeriod = await ParsePeriodAsync(query.PeriodStart);
            if (startPeriod > 0) queryable = queryable.Where(b => b.PeriodId >= startPeriod);
        }

        if (!string.IsNullOrEmpty(query.PeriodEnd))
        {
            var endPeriod = await ParsePeriodAsync(query.PeriodEnd);
            if (endPeriod > 0) queryable = queryable.Where(b => b.PeriodId <= endPeriod);
        }

        if (query.SubjectId.HasValue)
        {
            queryable = queryable.Where(b => b.SubjectId == query.SubjectId.Value);
        }

        var list = await queryable.ToListAsync();

        if (query.ShowZeroBalance == false)
        {
            list = list.Where(b => Math.Abs(b.EndDebit - b.EndCredit) > 0.01m).ToList();
        }

        return list;
    }

    /// <inheritdoc/>
    public async Task<PageResult<object>> GetDetailLedgerAsync(LedgerQuery query)
    {
        RefAsync<int> total = 0;
        var queryable = _db.Queryable<VoucherEntry>()
            .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
            .WhereIF(query.SubjectId.HasValue, e => e.SubjectId == query.SubjectId)
            .WhereIF(!string.IsNullOrEmpty(query.Keyword), e => e.Summary != null && e.Summary.Contains(query.Keyword!));

        if (query.DateStart.HasValue || query.DateEnd.HasValue)
        {
            var voucherQuery = _db.Queryable<Voucher>();
            if (query.DateStart.HasValue)
                voucherQuery = voucherQuery.Where(v => v.VoucherDate >= query.DateStart);
            if (query.DateEnd.HasValue)
                voucherQuery = voucherQuery.Where(v => v.VoucherDate <= query.DateEnd);
            var voucherIds = await voucherQuery.Select(v => v.Id).ToListAsync();
            queryable = queryable.Where(e => voucherIds.Contains(e.VoucherId));
        }

        var list = await queryable
            .OrderBy((e, v) => v.VoucherDate, OrderByType.Desc)
            .OrderBy((e, v) => v.VoucherNo)
            .Select((e, v) => new
            {
                e.Summary,
                e.SubjectId,
                e.DebitAmount,
                e.CreditAmount,
                VoucherNo = v.VoucherNo,
                VoucherDate = v.VoucherDate
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<object>(total, list.Cast<object>().ToList());
    }

    /// <inheritdoc/>
    public async Task<PageResult<object>> GetJournalAsync(LedgerQuery query)
    {
        var cashSubjects = await _db.Queryable<AccountSubject>()
            .Where(s => (s.IsCash == 1 || s.IsBank == 1) && s.IsEnabled == 1)
            .Select(s => s.Id)
            .ToListAsync();

        RefAsync<int> total = 0;
        var list = await _db.Queryable<VoucherEntry>()
            .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
            .Where(e => cashSubjects.Contains(e.SubjectId))
            .WhereIF(query.DateStart.HasValue, (e, v) => v.VoucherDate >= query.DateStart)
            .WhereIF(query.DateEnd.HasValue, (e, v) => v.VoucherDate <= query.DateEnd)
            .OrderBy((e, v) => v.VoucherDate)
            .Select((e, v) => new
            {
                e.Summary,
                e.SubjectId,
                e.DebitAmount,
                e.CreditAmount,
                VoucherNo = v.VoucherNo,
                VoucherDate = v.VoucherDate
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<object>(total, list.Cast<object>().ToList());
    }

    /// <inheritdoc/>
    public async Task<List<SubjectBalance>> GetSubjectSummaryAsync(int year, int month)
    {
        var period = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == year && p.PeriodMonth == month)
            ?? throw new NotFoundException("会计期间不存在");

        return await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == period.Id)
            .ToListAsync();
    }

    /// <summary>
    /// 解析期间字符串为期间ID
    /// </summary>
    private async Task<long> ParsePeriodAsync(string? periodStr)
    {
        if (string.IsNullOrEmpty(periodStr)) return 0;
        var parts = periodStr.Split('-');
        if (parts.Length != 2) return 0;
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);
        var period = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == year && p.PeriodMonth == month);
        return period?.Id ?? 0;
    }
}

/// <summary>
/// 辅助核算服务实现
/// </summary>
public class AuxiliaryService : IAuxiliaryService
{
    private readonly ISqlSugarClient _db;

    public AuxiliaryService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<object> GetListAsync(string type, AuxiliaryQuery query)
    {
        return type switch
        {
            "project" => await GetList<AuxProject>(query,
                q => q.WhereIF(!string.IsNullOrEmpty(query.Code), p => p.ProjectCode.Contains(query.Code!))
                     .WhereIF(!string.IsNullOrEmpty(query.Name), p => p.ProjectName.Contains(query.Name!))),
            "customer" => await GetList<AuxCustomer>(query,
                q => q.WhereIF(!string.IsNullOrEmpty(query.Code), c => c.CustomerCode.Contains(query.Code!))
                     .WhereIF(!string.IsNullOrEmpty(query.Name), c => c.CustomerName.Contains(query.Name!))),
            "supplier" => await GetList<AuxSupplier>(query,
                q => q.WhereIF(!string.IsNullOrEmpty(query.Code), s => s.SupplierCode.Contains(query.Code!))
                     .WhereIF(!string.IsNullOrEmpty(query.Name), s => s.SupplierName.Contains(query.Name!))),
            _ => throw new BusinessException($"不支持的辅助核算类型: {type}")
        };
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(string type, object request)
    {
        return type switch
        {
            "project" => await InsertEntity<AuxProject>(request),
            "customer" => await InsertEntity<AuxCustomer>(request),
            "supplier" => await InsertEntity<AuxSupplier>(request),
            _ => throw new BusinessException($"不支持的辅助核算类型: {type}")
        };
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(string type, long id, object request)
    {
        switch (type)
        {
            case "project": await UpdateEntity<AuxProject>(id, request); break;
            case "customer": await UpdateEntity<AuxCustomer>(id, request); break;
            case "supplier": await UpdateEntity<AuxSupplier>(id, request); break;
            default: throw new BusinessException($"不支持的辅助核算类型: {type}");
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string type, long id)
    {
        switch (type)
        {
            case "project": await _db.Deleteable<AuxProject>().Where(p => p.Id == id).ExecuteCommandAsync(); break;
            case "customer": await _db.Deleteable<AuxCustomer>().Where(c => c.Id == id).ExecuteCommandAsync(); break;
            case "supplier": await _db.Deleteable<AuxSupplier>().Where(s => s.Id == id).ExecuteCommandAsync(); break;
            default: throw new BusinessException($"不支持的辅助核算类型: {type}");
        }
    }

    private async Task<object> GetList<T>(AuxiliaryQuery query, Func<ISugarQueryable<T>, ISugarQueryable<T>> filter) where T : class, new()
    {
        RefAsync<int> total = 0;
        var queryable = filter(_db.Queryable<T>());
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<T>(total, list);
    }

    private async Task<long> InsertEntity<T>(object source) where T : class, new()
    {
        var entity = MapObject<T>(source);
        await _db.Insertable(entity).ExecuteCommandAsync();
        var idProp = typeof(T).GetProperty("Id");
        return (long)(idProp?.GetValue(entity) ?? 0);
    }

    private async Task UpdateEntity<T>(long id, object source) where T : class, new()
    {
        var entity = MapObject<T>(source);
        var idProp = typeof(T).GetProperty("Id");
        if (idProp != null) idProp.SetValue(entity, id);
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    private T MapObject<T>(object source) where T : class, new()
    {
        var target = new T();
        foreach (var sp in source.GetType().GetProperties())
        {
            var tp = typeof(T).GetProperty(sp.Name);
            if (tp != null && tp.CanWrite && tp.PropertyType == sp.PropertyType)
            {
                tp.SetValue(target, sp.GetValue(source));
            }
        }
        return target;
    }
}
