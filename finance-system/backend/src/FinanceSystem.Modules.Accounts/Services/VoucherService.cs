using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Accounts.Services;

/// <summary>
/// 凭证管理服务实现
/// </summary>
/// <summary>
/// 凭证服务实现
/// </summary>
public class VoucherService : IVoucherService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// <summary>
    /// 凭证管理服务
    /// </summary>
    public VoucherService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 分页查询凭证
    /// </summary>
    public async Task<PageResult<Voucher>> GetPageAsync(VoucherQuery query)
    {
        RefAsync<int> total = 0;
        var queryable = _db.Queryable<Voucher>()
            .WhereIF(!string.IsNullOrEmpty(query.VoucherNo), v => v.VoucherNo.Contains(query.VoucherNo!))
            .WhereIF(query.DateStart.HasValue, v => v.VoucherDate >= query.DateStart)
            .WhereIF(query.DateEnd.HasValue, v => v.VoucherDate <= query.DateEnd)
            .WhereIF(query.VoucherType.HasValue, v => v.VoucherType == query.VoucherType)
            .WhereIF(query.Status.HasValue, v => v.Status == query.Status);

        // 按摘要关键词筛选（需子查询凭证分录）
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            var voucherIds = await _db.Queryable<VoucherEntry>()
                .Where(e => e.Summary != null && e.Summary.Contains(query.Keyword))
                .Select(e => e.VoucherId).ToListAsync();
            queryable = queryable.Where(v => voucherIds.Contains(v.Id));
        }

        // 按科目筛选
        if (query.SubjectId.HasValue)
        {
            var voucherIds2 = await _db.Queryable<VoucherEntry>()
                .Where(e => e.SubjectId == query.SubjectId.Value)
                .Select(e => e.VoucherId).ToListAsync();
            queryable = queryable.Where(v => voucherIds2.Contains(v.Id));
        }

        var list = await queryable
            .OrderBy(v => v.VoucherDate, OrderByType.Desc)
            .OrderBy(v => v.VoucherNo)
            .ApplySort(query.SortField, query.SortOrder).ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<Voucher>(total, list);
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 获取详情
    /// </summary>
    public async Task<Voucher?> GetByIdAsync(long id)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id);
        if (voucher != null)
        {
            voucher.Entries = await _db.Queryable<VoucherEntry>()
                .Where(e => e.VoucherId == id)
                .OrderBy(e => e.Id)
                .ToListAsync();

            var subjectIds = voucher.Entries.Select(e => e.SubjectId).Distinct().ToList();
            var subjects = await _db.Queryable<AccountSubject>()
                .Where(s => subjectIds.Contains(s.Id))
                .ToListAsync();
            foreach (var entry in voucher.Entries)
            {
                entry.Subject = subjects.FirstOrDefault(s => s.Id == entry.SubjectId);
            }
        }
        return voucher;
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(VoucherCreateRequest request, long currentUserId)
    {
        if (request.Entries == null || request.Entries.Count < 2)
            throw new BusinessException("凭证至少包含2条分录");

        var totalDebit = request.Entries.Sum(e => e.DebitAmount);
        var totalCredit = request.Entries.Sum(e => e.CreditAmount);
        if (Math.Abs(totalDebit - totalCredit) > 0.01m)
            throw new BusinessException($"借贷不平衡，借方合计{totalDebit}，贷方合计{totalCredit}");

        var period = await _db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.BeginDate <= request.VoucherDate && p.EndDate >= request.VoucherDate && p.IsClosed == 0)
            ?? throw new BusinessException("凭证日期不在当前未关闭的会计期间内");

        var voucherNo = await GenerateVoucherNoAsync(period.Id, request.VoucherType);

        var voucher = new Voucher
        {
            VoucherNo = voucherNo,
            VoucherDate = request.VoucherDate,
            PeriodId = period.Id,
            VoucherType = request.VoucherType,
            AbstractText = request.AbstractText,
            Status = 0,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            PreparedBy = currentUserId
        };

        await _db.Insertable(voucher).ExecuteCommandAsync();

        var entries = request.Entries.Select(e => new VoucherEntry
        {
            VoucherId = voucher.Id,
            Summary = e.Summary,
            SubjectId = e.SubjectId,
            DebitAmount = e.DebitAmount,
            CreditAmount = e.CreditAmount,
            AuxiliaryId = e.AuxiliaryId,
            AuxiliaryType = e.AuxiliaryType
        }).ToList();
        await _db.Insertable(entries).ExecuteCommandAsync();

        return voucher.Id;
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 修改
    /// </summary>
    public async Task UpdateAsync(long id, VoucherCreateRequest request)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");

        if (voucher.Status != 0) throw new BusinessException("仅草稿状态的凭证可修改");

        var totalDebit = request.Entries.Sum(e => e.DebitAmount);
        var totalCredit = request.Entries.Sum(e => e.CreditAmount);
        if (Math.Abs(totalDebit - totalCredit) > 0.01m)
            throw new BusinessException($"借贷不平衡，借方合计{totalDebit}，贷方合计{totalCredit}");

        voucher.VoucherDate = request.VoucherDate;
        voucher.VoucherType = request.VoucherType;
        voucher.AbstractText = request.AbstractText;
        voucher.TotalDebit = totalDebit;
        voucher.TotalCredit = totalCredit;

        await _db.Updateable(voucher).ExecuteCommandAsync();

        await _db.Deleteable<VoucherEntry>().Where(e => e.VoucherId == id).ExecuteCommandAsync();
        var entries = request.Entries.Select(e => new VoucherEntry
        {
            VoucherId = id,
            Summary = e.Summary,
            SubjectId = e.SubjectId,
            DebitAmount = e.DebitAmount,
            CreditAmount = e.CreditAmount,
            AuxiliaryId = e.AuxiliaryId,
            AuxiliaryType = e.AuxiliaryType
        }).ToList();
        await _db.Insertable(entries).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 审核凭证
    /// </summary>
    public async Task AuditAsync(long id, long currentUserId)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");

        if (voucher.Status != 0) throw new BusinessException("仅草稿状态的凭证可审核");
        if (voucher.PreparedBy == currentUserId) throw new BusinessException("制单人与审核人不可为同一人");

        voucher.Status = 1;
        voucher.ReviewedBy = currentUserId;
        voucher.ReviewedTime = DateTime.Now;

        await _db.Updateable(voucher)
            .UpdateColumns(v => new { v.Status, v.ReviewedBy, v.ReviewedTime })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 反审核凭证
    /// </summary>
    public async Task UnAuditAsync(long id)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");

        if (voucher.Status != 1) throw new BusinessException("仅已审核的凭证可反审核");

        var period = await _db.Queryable<AccountingPeriod>().FirstAsync(p => p.Id == voucher.PeriodId);
        if (period != null && period.IsClosed == 1)
            throw new BusinessException("凭证所在会计期间已结账，不可反审核");

        voucher.Status = 0;
        voucher.ReviewedBy = null;
        voucher.ReviewedTime = null;

        await _db.Updateable(voucher)
            .UpdateColumns(v => new { v.Status, v.ReviewedBy, v.ReviewedTime })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 作废凭证
    /// </summary>
    public async Task VoidAsync(long id)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");

        if (voucher.Status == 2) throw new BusinessException("凭证已作废，不可重复作废");

        voucher.Status = 2;
        await _db.Updateable(voucher).UpdateColumns(v => v.Status).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量审核凭证
    /// </summary>
    public async Task BatchAuditAsync(List<long> ids, long currentUserId)
    {
        var vouchers = await _db.Queryable<Voucher>().Where(v => ids.Contains(v.Id)).ToListAsync();
        var errors = new List<string>();

        foreach (var voucher in vouchers)
        {
            if (voucher.Status != 0) { errors.Add($"凭证{voucher.VoucherNo}非草稿状态"); continue; }
            if (voucher.PreparedBy == currentUserId) { errors.Add($"凭证{voucher.VoucherNo}制单人与审核人相同"); continue; }
            voucher.Status = 1;
            voucher.ReviewedBy = currentUserId;
            voucher.ReviewedTime = DateTime.Now;
        }

        if (errors.Any()) throw new BusinessException(string.Join("；", errors));
        if (vouchers.Any()) await _db.Updateable(vouchers).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量作废凭证
    /// </summary>
    public async Task BatchVoidAsync(List<long> ids)
    {
        var vouchers = await _db.Queryable<Voucher>().Where(v => ids.Contains(v.Id) && v.Status == 2).ToListAsync();
        if (vouchers.Any()) throw new BusinessException("存在已作废凭证，不可重复作废");
        await _db.Updateable<Voucher>().SetColumns(v => v.Status == 2)
            .Where(v => ids.Contains(v.Id) && v.Status != 2).ExecuteCommandAsync();
    }

    /// <summary>
    /// 红字冲销：生成原凭证的借贷反向凭证，摘要标注冲销来源
    /// </summary>
    public async Task<long> ReverseAsync(long originalId, long currentUserId)
    {
        var original = await _db.Queryable<Voucher>()
            .Includes(v => v.Entries)
            .FirstAsync(v => v.Id == originalId)
            ?? throw new NotFoundException("原凭证不存在");
        if (original.Status != 2) throw new BusinessException("仅能冲销已作废的凭证");

        var reversalVoucher = new Voucher
        {
            VoucherNo = await GenerateVoucherNoAsync(original.PeriodId, original.VoucherType),
            VoucherType = original.VoucherType,
            PeriodId = original.PeriodId,
            VoucherDate = DateTime.Now,
            AbstractText = $"冲销凭证{original.VoucherNo}",
            PreparedBy = currentUserId,
            Status = 0
        };
        await _db.Insertable(reversalVoucher).ExecuteCommandAsync();

        var reversalEntries = (original.Entries ?? new List<VoucherEntry>()).Select(e => new VoucherEntry
        {
            VoucherId = reversalVoucher.Id,
            SubjectId = e.SubjectId,
            Summary = $"冲销：{e.Summary}",
            DebitAmount = e.CreditAmount,
            CreditAmount = e.DebitAmount,
            AuxiliaryId = e.AuxiliaryId,
            AuxiliaryType = e.AuxiliaryType
        }).ToList();
        await _db.Insertable(reversalEntries).ExecuteCommandAsync();

        return reversalVoucher.Id;
    }

    /// <summary>
    /// 生成凭证号（按期间+类型递增）
    /// </summary>
    private async Task<string> GenerateVoucherNoAsync(long periodId, int voucherType)
    {
        var prefix = voucherType switch { 1 => "SK", 2 => "FK", _ => "ZZ" };
        var count = await _db.Queryable<Voucher>()
            .Where(v => v.PeriodId == periodId && v.VoucherType == voucherType)
            .CountAsync() + 1;
        return $"{prefix}-{count:D4}";
    }

    /// <summary>
    /// 复制凭证（生成草稿副本）
    /// </summary>
    public async Task<long> CopyAsync(long id, long currentUserId)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");

        // 生成新凭证号
        var newNo = await GenerateVoucherNoAsync(voucher.PeriodId, voucher.VoucherType);

        var newVoucher = new Voucher
        {
            VoucherNo = newNo,
            VoucherDate = DateTime.Today,
            PeriodId = voucher.PeriodId,
            VoucherType = voucher.VoucherType,
            AbstractText = voucher.AbstractText + "（复制）",
            Status = 0, // 草稿
            TotalDebit = voucher.TotalDebit,
            TotalCredit = voucher.TotalCredit,
            PreparedBy = currentUserId,
            CreatedTime = DateTime.Now
        };

        await _db.Insertable(newVoucher).ExecuteCommandAsync();

        // 复制分录
        var entries = await _db.Queryable<VoucherEntry>()
            .Where(e => e.VoucherId == id).ToListAsync();

        var newEntries = entries.Select(e => new VoucherEntry
        {
            VoucherId = newVoucher.Id,
            Summary = e.Summary,
            SubjectId = e.SubjectId,
            DebitAmount = e.DebitAmount,
            CreditAmount = e.CreditAmount,
            AuxiliaryType = e.AuxiliaryType,
            AuxiliaryId = e.AuxiliaryId,
            CreatedTime = DateTime.Now
        }).ToList();

        await _db.Insertable(newEntries).ExecuteCommandAsync();

        return newVoucher.Id;
    }

    /// <summary>
    /// 删除凭证（仅草稿状态可删除）
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var voucher = await _db.Queryable<Voucher>().FirstAsync(v => v.Id == id)
            ?? throw new NotFoundException("凭证不存在");
        if (voucher.Status != 0)
            throw new InvalidOperationException("仅草稿状态的凭证可以删除");

        await _db.Deleteable<VoucherEntry>().Where(e => e.VoucherId == id).ExecuteCommandAsync();
        await _db.Deleteable<Voucher>().Where(v => v.Id == id).ExecuteCommandAsync();
    }
}
