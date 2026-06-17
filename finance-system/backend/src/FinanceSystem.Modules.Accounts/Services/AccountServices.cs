using System;
using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Accounts.Services;

/// <summary>
/// 会计期间服务实现
/// </summary>
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

        // 结账前检查：当期所有凭证必须已审核
        var hasDraft = await _db.Queryable<Voucher>()
            .AnyAsync(v => v.PeriodId == periodId && v.Status == 0);
        if (hasDraft) throw new BusinessException("当期存在未审核的凭证，请先审核所有凭证");

        // 结账前检查：试算平衡
        var balances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == periodId).ToListAsync();
        var totalDebit = balances.Sum(b => b.EndDebit);
        var totalCredit = balances.Sum(b => b.EndCredit);
        if (Math.Abs(totalDebit - totalCredit) > 0.01m)
            throw new BusinessException($"试算不平衡：借方合计{totalDebit}，贷方合计{totalCredit}，差额{Math.Abs(totalDebit - totalCredit)}");

        // 年末结账（12月）：自动结转本年利润到利润分配
        if (period.PeriodMonth == 12)
        {
            await YearEndCloseAsync(period, currentUserId);
        }

        // 结账后自动将本期期末余额滚入下一期作为期初余额
        await CarryForwardBalancesAsync(periodId);

        period.IsClosed = 1;
        period.ClosedTime = DateTime.Now;
        period.ClosedBy = currentUserId;

        await _db.Updateable(period)
            .UpdateColumns(p => new { p.IsClosed, p.ClosedTime, p.ClosedBy })
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 年末结转：将本年利润结转到利润分配-未分配利润
    /// </summary>
    private async Task YearEndCloseAsync(AccountingPeriod period, long currentUserId)
    {
        var profitSubject = await _db.Queryable<AccountSubject>()
            .FirstAsync(s => s.SubjectCode == "4103")
            ?? throw new BusinessException("本年利润科目（4103）不存在");

        var retainedSubject = await _db.Queryable<AccountSubject>()
            .FirstAsync(s => s.SubjectCode == "4104")
            ?? throw new BusinessException("利润分配科目（4104）不存在");

        // 查询本年利润科目在12月的余额
        var profitBalance = await _db.Queryable<SubjectBalance>()
            .FirstAsync(b => b.SubjectId == profitSubject.Id && b.PeriodId == period.Id);

        if (profitBalance == null || (Math.Abs(profitBalance.EndDebit) < 0.01m && Math.Abs(profitBalance.EndCredit) < 0.01m))
        {
            // 本年利润为零，无需结转
            return;
        }

        // 计算净利润：本年利润贷方余额=盈利，借方余额=亏损
        decimal netProfit = profitBalance.EndCredit - profitBalance.EndDebit;

        // 生成凭证号
        var lastVoucher = await _db.Queryable<Voucher>()
            .Where(v => v.PeriodId == period.Id).OrderByDescending(v => v.Id).FirstAsync();
        var nextNo = lastVoucher != null
            ? (int.Parse(lastVoucher.VoucherNo.Split('-').Last()) + 1).ToString("D4")
            : "0001";

        var entries = new List<VoucherEntry>();
        decimal debitAmt = 0, creditAmt = 0;

        if (netProfit > 0)
        {
            // 盈利：借记本年利润，贷记利润分配
            entries.Add(new VoucherEntry
            {
                VoucherId = 0, SubjectId = profitSubject.Id,
                Summary = "年末结转-本年利润", DebitAmount = netProfit, CreditAmount = 0
            });
            entries.Add(new VoucherEntry
            {
                VoucherId = 0, SubjectId = retainedSubject.Id,
                Summary = "年末结转-利润分配", DebitAmount = 0, CreditAmount = netProfit
            });
            debitAmt = netProfit;
            creditAmt = netProfit;
        }
        else if (netProfit < 0)
        {
            // 亏损：借记利润分配，贷记本年利润
            var lossAmt = Math.Abs(netProfit);
            entries.Add(new VoucherEntry
            {
                VoucherId = 0, SubjectId = retainedSubject.Id,
                Summary = "年末结转-利润分配（亏损）", DebitAmount = lossAmt, CreditAmount = 0
            });
            entries.Add(new VoucherEntry
            {
                VoucherId = 0, SubjectId = profitSubject.Id,
                Summary = "年末结转-本年利润（亏损）", DebitAmount = 0, CreditAmount = lossAmt
            });
            debitAmt = lossAmt;
            creditAmt = lossAmt;
        }

        if (entries.Any())
        {
            var voucher = new Voucher
            {
                VoucherNo = $"PZ-{period.PeriodYear}{period.PeriodMonth:D2}-{nextNo}",
                VoucherDate = period.EndDate,
                PeriodId = period.Id,
                VoucherType = 2,
                AbstractText = "年末结转本年利润",
                Status = 1,
                TotalDebit = debitAmt,
                TotalCredit = creditAmt,
                PreparedBy = currentUserId,
                ReviewedBy = currentUserId,
                ReviewedTime = DateTime.Now,
                Entries = entries
            };
            await _db.Insertable(voucher).ExecuteCommandAsync();

            // 自动初始化下一年度会计期间
            var nextYear = await _db.Queryable<AccountingPeriod>()
                .FirstAsync(p => p.PeriodYear == period.PeriodYear + 1);
            if (nextYear == null)
            {
                await InitYearAsync(period.PeriodYear + 1);
            }
        }
    }

    /// <summary>
    /// 结账后将本期科目余额期末数据滚入下一期
    /// </summary>
    private async Task CarryForwardBalancesAsync(long periodId)
    {
        var period = await _db.Queryable<AccountingPeriod>().FirstAsync(p => p.Id == periodId);
        if (period == null) return;

        var nextPeriod = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == period.PeriodYear && p.PeriodMonth == period.PeriodMonth + 1);
        if (nextPeriod == null) return;

        // 查询本期科目余额
        var currentBalances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == periodId).ToListAsync();

        // 查询下一期已有余额（可能已手动录入期初余额）
        var nextBalances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == nextPeriod.Id).ToListAsync();

        var upsertList = new List<SubjectBalance>();

        foreach (var cb in currentBalances)
        {
            // 仅滚转有余额的科目（资产/负债/权益类，不滚转损益类）
            if (Math.Abs(cb.EndDebit) < 0.01m && Math.Abs(cb.EndCredit) < 0.01m) continue;

            // 检查科目类型（损益类科目不滚转）
            var subject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.Id == cb.SubjectId);
            if (subject == null || subject.SubjectType == 4 || subject.SubjectType == 5) continue;

            var existing = nextBalances.FirstOrDefault(b => b.SubjectId == cb.SubjectId);
            if (existing != null)
            {
                existing.BeginDebit = cb.EndDebit;
                existing.BeginCredit = cb.EndCredit;
                existing.EndDebit = cb.EndDebit;
                existing.EndCredit = cb.EndCredit;
                existing.CurrentDebit = 0;
                existing.CurrentCredit = 0;
                upsertList.Add(existing);
            }
            else
            {
                upsertList.Add(new SubjectBalance
                {
                    SubjectId = cb.SubjectId,
                    PeriodId = nextPeriod.Id,
                    BeginDebit = cb.EndDebit,
                    BeginCredit = cb.EndCredit,
                    CurrentDebit = 0,
                    CurrentCredit = 0,
                    EndDebit = cb.EndDebit,
                    EndCredit = cb.EndCredit
                });
            }
        }

        if (upsertList.Any())
        {
            await _db.Storageable(upsertList)
                .WhereColumns(b => new { b.SubjectId, b.PeriodId })
                .ToStorage()
                .AsInsertable.ExecuteCommandAsync();
        }
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

        // 查询损益类科目（类型4收入、5费用、6成本）的当期余额
        var balances = await _db.Queryable<SubjectBalance>()
            .Where(b => b.PeriodId == periodId)
            .ToListAsync();

        var subjectIds = balances.Select(b => b.SubjectId).ToList();
        var subjects = await _db.Queryable<AccountSubject>()
            .Where(s => subjectIds.Contains(s.Id))
            .ToListAsync();

        var profitLossBalances = balances
            .Join(subjects, b => b.SubjectId, s => s.Id, (b, s) => new { Balance = b, Subject = s })
            .Where(x => x.Subject.SubjectType == 4 || x.Subject.SubjectType == 5 || x.Subject.SubjectType == 6)
            .ToList();

        if (!profitLossBalances.Any()) throw new BusinessException("当期无损益科目余额需要结转");

        // 计算各损益科目的净额（收入类贷方余额=贷方-借方，费用类借方余额=借方-贷方）
        var entries = new List<VoucherEntry>();
        decimal totalDebit = 0;
        decimal totalCredit = 0;

        foreach (var item in profitLossBalances)
        {
            decimal netAmount;
            if (item.Subject.SubjectType == 4)
            {
                // 收入类：贷方余额 = 贷方 - 借方，结转时借记收入、贷记本年利润
                netAmount = item.Balance.EndCredit - item.Balance.EndDebit;
                if (netAmount > 0)
                {
                    entries.Add(new VoucherEntry
                    {
                        VoucherId = 0,
                        SubjectId = item.Subject.Id,
                        Summary = $"期末损益结转-{item.Subject.SubjectName}",
                        DebitAmount = netAmount,
                        CreditAmount = 0
                    });
                    totalDebit += netAmount;
                }
            }
            else
            {
                // 费用/成本类：借方余额 = 借方 - 贷方，结转时贷记费用、借记本年利润
                netAmount = item.Balance.EndDebit - item.Balance.EndCredit;
                if (netAmount > 0)
                {
                    entries.Add(new VoucherEntry
                    {
                        VoucherId = 0,
                        SubjectId = item.Subject.Id,
                        Summary = $"期末损益结转-{item.Subject.SubjectName}",
                        CreditAmount = netAmount,
                        DebitAmount = 0
                    });
                    totalCredit += netAmount;
                }
            }
        }

        // 找到本年利润科目（4103）
        var profitSubject = await _db.Queryable<AccountSubject>()
            .FirstAsync(s => s.SubjectCode == "4103")
            ?? throw new BusinessException("本年利润科目（4103）不存在");

        // 收入结转贷记本年利润
        entries.Add(new VoucherEntry
        {
            VoucherId = 0,
            SubjectId = profitSubject.Id,
            Summary = "期末损益结转-本年利润（收入结转）",
            CreditAmount = totalDebit,
            DebitAmount = 0
        });

        // 费用结转借记本年利润
        entries.Add(new VoucherEntry
        {
            VoucherId = 0,
            SubjectId = profitSubject.Id,
            Summary = "期末损益结转-本年利润（费用结转）",
            DebitAmount = totalCredit,
            CreditAmount = 0
        });

        // 生成凭证号
        var lastVoucher = await _db.Queryable<Voucher>()
            .Where(v => v.PeriodId == periodId)
            .OrderByDescending(v => v.Id)
            .FirstAsync();
        var nextNo = lastVoucher != null ? ((int.Parse(lastVoucher.VoucherNo.Split('-').Last()) + 1).ToString("D4")) : "0001";

        // 创建结转凭证
        var voucher = new Voucher
        {
            VoucherNo = $"PZ-{period.PeriodYear}{period.PeriodMonth:D2}-{nextNo}",
            VoucherDate = period.EndDate,
            PeriodId = periodId,
            VoucherType = 2,
            AbstractText = "期末损益结转",
            Status = 1,
            TotalDebit = Math.Max(totalDebit, totalCredit),
            TotalCredit = Math.Max(totalDebit, totalCredit),
            PreparedBy = currentUserId,
            ReviewedBy = currentUserId,
            ReviewedTime = DateTime.Now,
            Entries = entries
        };

        await _db.Insertable(voucher).ExecuteCommandAsync();
    }
}

/// <summary>
/// 科目期初余额服务实现
/// </summary>
/// <summary>
/// 科目余额服务实现
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
            .LeftJoin<AccountSubject>((e, v, s) => e.SubjectId == s.Id)
            .OrderBy((e, v) => v.VoucherDate, OrderByType.Desc)
            .OrderBy((e, v) => v.VoucherNo)
            .Select((e, v, s) => new
            {
                e.Summary,
                e.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
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
            .LeftJoin<AccountSubject>((e, v, s) => e.SubjectId == s.Id)
            .Where(e => cashSubjects.Contains(e.SubjectId))
            .WhereIF(query.DateStart.HasValue, (e, v) => v.VoucherDate >= query.DateStart)
            .WhereIF(query.DateEnd.HasValue, (e, v) => v.VoucherDate <= query.DateEnd)
            .OrderBy((e, v) => v.VoucherDate)
            .Select((e, v, s) => new
            {
                e.Summary,
                e.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
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
        var list = await queryable.ApplySort(query.SortField, query.SortOrder)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
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
