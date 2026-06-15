using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Asset.DTOs;
using FinanceSystem.Modules.Asset.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Asset.Services;

/// <summary>资产分类服务接口</summary>
public interface IAssetCategoryService
{
    Task<List<AssetCategory>> GetTreeAsync();
    Task<long> CreateAsync(AssetCategoryRequest request);
    Task UpdateAsync(long id, AssetCategoryRequest request);
    Task DeleteAsync(long id);
}

/// <summary>资产卡片服务接口</summary>
public interface IAssetCardService
{
    Task<PageResult<AssetCard>> GetPageAsync(AssetCardQuery query);
    Task<AssetCard?> GetByIdAsync(long id);
    Task<long> CreateAsync(AssetCardRequest request);
    Task UpdateAsync(long id, AssetCardRequest request);
    Task ChangeStatusAsync(long id, int changeType, AssetChangeRequest request, long currentUserId);
    Task<List<AssetCard>> GetDepreciableListAsync(int month);
    /// <summary>资产处置（报废/出售）并自动生成凭证</summary>
    Task<long> DisposeAsync(long id, AssetDisposeRequest request, long currentUserId);
}

/// <summary>资产折旧服务接口</summary>
public interface IAssetDepreciationService
{
    Task<List<AssetDepreciation>> CalculateAsync(int year, int month);
    Task ConfirmDepreciationAsync(int year, int month, long currentUserId);
    Task<List<object>> GetSummaryAsync(int year);
}

/// <summary>资产盘点服务接口</summary>
public interface IAssetInventoryService
{
    /// <summary>创建盘点单</summary>
    Task<long> CreateAsync(AssetInventoryRequest request);
    /// <summary>分页查询盘点单</summary>
    Task<PageResult<AssetInventory>> GetListAsync(PageRequest query);
    /// <summary>完成盘点</summary>
    Task CompleteAsync(long id);
}

/// <summary>资产报表服务接口</summary>
public interface IAssetReportService
{
    /// <summary>资产台账</summary>
    Task<PageResult<object>> GetLedgerAsync(AssetReportQuery query);
    /// <summary>折旧汇总表</summary>
    Task<List<object>> GetDepreciationSummaryAsync(int year);
    /// <summary>资产价值统计</summary>
    Task<object> GetValueStatsAsync();
}

/// <summary>资产分类服务实现</summary>
public class AssetCategoryService : IAssetCategoryService
{
    private readonly ISqlSugarClient _db;
    public AssetCategoryService(ISqlSugarClient db) => _db = db;

    public async Task<List<AssetCategory>> GetTreeAsync()
    {
        var all = await _db.Queryable<AssetCategory>().OrderBy(c => c.SortOrder).ToListAsync();
        return BuildTree(all, 0);
    }

    public async Task<long> CreateAsync(AssetCategoryRequest request)
    {
        var entity = new AssetCategory
        {
            ParentId = request.ParentId, CategoryCode = request.CategoryCode,
            CategoryName = request.CategoryName, DepreciationMethod = request.DepreciationMethod,
            UsefulLifeMonths = request.UsefulLifeMonths, ResidualRate = request.ResidualRate,
            SortOrder = request.SortOrder
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(long id, AssetCategoryRequest request)
    {
        var entity = await _db.Queryable<AssetCategory>().FirstAsync(c => c.Id == id)
            ?? throw new NotFoundException("资产分类不存在");
        entity.CategoryName = request.CategoryName;
        entity.DepreciationMethod = request.DepreciationMethod;
        entity.UsefulLifeMonths = request.UsefulLifeMonths;
        entity.ResidualRate = request.ResidualRate;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var hasAsset = await _db.Queryable<AssetCard>().AnyAsync(a => a.CategoryId == id);
        if (hasAsset) throw new BusinessException("该分类下有资产，不可删除");
        await _db.Deleteable<AssetCategory>().Where(c => c.Id == id).ExecuteCommandAsync();
    }

    private List<AssetCategory> BuildTree(List<AssetCategory> all, long parentId)
        => all.Where(c => c.ParentId == parentId).Select(c => { return c; }).ToList();
}

/// <summary>资产卡片服务实现</summary>
public class AssetCardService : IAssetCardService
{
    private readonly ISqlSugarClient _db;
    public AssetCardService(ISqlSugarClient db) => _db = db;

    public async Task<PageResult<AssetCard>> GetPageAsync(AssetCardQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<AssetCard>()
            .WhereIF(!string.IsNullOrEmpty(query.AssetCode), a => a.AssetCode.Contains(query.AssetCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetName), a => a.AssetName.Contains(query.AssetName!))
            .WhereIF(query.CategoryId.HasValue, a => a.CategoryId == query.CategoryId)
            .WhereIF(query.DeptId.HasValue, a => a.DeptId == query.DeptId)
            .WhereIF(query.Status.HasValue, a => a.Status == query.Status)
            .OrderBy(a => a.AssetCode)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<AssetCard>(total, list);
    }

    public async Task<AssetCard?> GetByIdAsync(long id)
    {
        return await _db.Queryable<AssetCard>().FirstAsync(a => a.Id == id);
    }

    public async Task<long> CreateAsync(AssetCardRequest request)
    {
        var category = await _db.Queryable<AssetCategory>().FirstAsync(c => c.Id == request.CategoryId)
            ?? throw new NotFoundException("资产分类不存在");

        var residualValue = Math.Round(request.OriginalValue * request.ResidualRate / 100, 2);
        var ym = DateTime.Now.ToString("yyyyMM");
        var count = await _db.Queryable<AssetCard>().Where(a => a.AssetCode.Contains(ym)).CountAsync() + 1;
        var assetCode = $"{category.CategoryCode}-{ym}-{count:D4}";

        var entity = new AssetCard
        {
            AssetCode = assetCode, AssetName = request.AssetName, CategoryId = request.CategoryId,
            Specification = request.Specification, OriginalValue = request.OriginalValue,
            ResidualRate = request.ResidualRate, ResidualValue = residualValue,
            DepreciationMethod = request.DepreciationMethod, UsefulLifeMonths = request.UsefulLifeMonths,
            AcquisitionDate = request.AcquisitionDate, DeptId = request.DeptId,
            Keeper = request.Keeper, Location = request.Location, Status = 1,
            AccumulatedDepreciation = 0, NetValue = request.OriginalValue - residualValue,
            Remark = request.Remark
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(long id, AssetCardRequest request)
    {
        var entity = await _db.Queryable<AssetCard>().FirstAsync(a => a.Id == id)
            ?? throw new NotFoundException("资产不存在");
        if (entity.AccumulatedDepreciation > 0) throw new BusinessException("已计提折旧的资产不可修改原值和年限");

        entity.AssetName = request.AssetName;
        entity.Specification = request.Specification;
        entity.DeptId = request.DeptId;
        entity.Keeper = request.Keeper;
        entity.Location = request.Location;
        entity.Remark = request.Remark;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task ChangeStatusAsync(long id, int changeType, AssetChangeRequest request, long currentUserId)
    {
        var asset = await _db.Queryable<AssetCard>().FirstAsync(a => a.Id == id)
            ?? throw new NotFoundException("资产不存在");

        int newStatus = changeType switch { 1 => 1, 2 => 4, 3 => 5, _ => throw new BusinessException("无效的变动类型") };

        var change = new AssetChange
        {
            AssetCardId = id, ChangeType = changeType, Reason = request.Reason,
            FromDeptId = asset.DeptId, ToDeptId = request.ToDeptId,
            DisposalIncome = request.DisposalIncome, OperatorId = currentUserId
        };
        await _db.Insertable(change).ExecuteCommandAsync();

        asset.Status = newStatus;
        if (request.ToDeptId.HasValue) asset.DeptId = request.ToDeptId;
        await _db.Updateable(asset).UpdateColumns(a => new { a.Status, a.DeptId }).ExecuteCommandAsync();
    }

    public async Task<List<AssetCard>> GetDepreciableListAsync(int month)
    {
        return await _db.Queryable<AssetCard>()
            .Where(a => a.Status == 1 && a.AccumulatedDepreciation < (a.OriginalValue - a.ResidualValue))
            .ToListAsync();
    }

    public async Task<long> DisposeAsync(long id, AssetDisposeRequest request, long currentUserId)
    {
        return await AssetDisposeHelper.DisposeAssetAsync(_db, id, request, currentUserId);
    }
}

/// <summary>资产处置服务</summary>
public static class AssetDisposeHelper
{
    public static async Task<long> DisposeAssetAsync(ISqlSugarClient db, long id, AssetDisposeRequest request, long currentUserId)
    {
        var card = await db.Queryable<AssetCard>().FirstAsync(c => c.Id == id)
            ?? throw new NotFoundException("资产卡片不存在");
        if (card.Status != 1 && card.Status != 2)
            throw new BusinessException("仅使用中或闲置资产可处置");

        card.Status = request.DisposeType;
        card.UpdatedTime = DateTime.Now;
        await db.Updateable(card).UpdateColumns(c => new { c.Status, c.UpdatedTime }).ExecuteCommandAsync();

        decimal netValue = card.OriginalValue - card.AccumulatedDepreciation;
        decimal disposalIncome = request.DisposalIncome;
        decimal lossOrGain = disposalIncome - netValue;

        var now = request.DisposeDate;
        var period = await db.Queryable<AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == now.Year && p.PeriodMonth == now.Month);
        if (period == null) throw new BusinessException("当前月份会计期间不存在");

        // 查找固定资产科目（1601）
        var fixedAssetSubject = await db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "1601");
        if (fixedAssetSubject == null) throw new BusinessException("固定资产科目(1601)不存在");

        var entries = new List<VoucherEntry>();

        // 转出累计折旧
        if (card.AccumulatedDepreciation > 0)
        {
            var accSubj = await db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "1602");
            if (accSubj != null)
            {
                entries.Add(new VoucherEntry
                {
                    SubjectId = accSubj.Id, Summary = $"处置资产{card.AssetName}转出累计折旧",
                    DebitAmount = card.AccumulatedDepreciation, CreditAmount = 0
                });
                entries.Add(new VoucherEntry
                {
                    SubjectId = fixedAssetSubject.Id, Summary = $"处置资产{card.AssetName}转出原值",
                    DebitAmount = 0, CreditAmount = card.AccumulatedDepreciation
                });
            }
        }

        // 处置收入
        if (disposalIncome > 0)
        {
            var bankSubject = await db.Queryable<AccountSubject>().FirstAsync(s => s.IsBank == 1);
            if (bankSubject != null)
            {
                entries.Add(new VoucherEntry
                {
                    SubjectId = bankSubject.Id, Summary = $"处置资产{card.AssetName}收入",
                    DebitAmount = disposalIncome, CreditAmount = 0
                });
            }
        }

        // 处置损益（净收益→营业外收入，净损失→营业外支出）
        if (lossOrGain > 0)
        {
            var incSubj = await db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "6301");
            if (incSubj != null)
            {
                entries.Add(new VoucherEntry
                {
                    SubjectId = fixedAssetSubject.Id, Summary = $"处置资产{card.AssetName}结转",
                    DebitAmount = 0, CreditAmount = netValue
                });
                entries.Add(new VoucherEntry
                {
                    SubjectId = incSubj.Id, Summary = $"处置资产{card.AssetName}净收益",
                    DebitAmount = 0, CreditAmount = lossOrGain
                });
            }
        }
        else if (lossOrGain < 0)
        {
            var expSubj = await db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "6711");
            if (expSubj != null)
            {
                entries.Add(new VoucherEntry
                {
                    SubjectId = fixedAssetSubject.Id, Summary = $"处置资产{card.AssetName}结转",
                    DebitAmount = 0, CreditAmount = netValue
                });
                entries.Add(new VoucherEntry
                {
                    SubjectId = expSubj.Id, Summary = $"处置资产{card.AssetName}净损失",
                    DebitAmount = Math.Abs(lossOrGain), CreditAmount = 0
                });
            }
        }

        if (!entries.Any()) return 0;

        // 生成处置凭证
        var voucherCount = await db.Queryable<Voucher>().Where(v => v.PeriodId == period.Id).CountAsync();
        var voucher = new Voucher
        {
            VoucherNo = $"ZZ-{voucherCount + 1:D4}", VoucherType = 0, PeriodId = period.Id, VoucherDate = now,
            AbstractText = $"资产处置-{card.AssetName}", PreparedBy = currentUserId, Status = 0
        };
        await db.Insertable(voucher).ExecuteCommandAsync();
        foreach (var e in entries) e.VoucherId = voucher.Id;
        await db.Insertable(entries).ExecuteCommandAsync();
        return voucher.Id;
    }
}

/// <summary>资产折旧服务实现</summary>
public class AssetDepreciationService : IAssetDepreciationService
{
    private readonly ISqlSugarClient _db;
    public AssetDepreciationService(ISqlSugarClient db) => _db = db;

    public async Task<List<AssetDepreciation>> CalculateAsync(int year, int month)
    {
        var period = await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == year && p.PeriodMonth == month);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var assets = await new AssetCardService(_db).GetDepreciableListAsync(month);
        var result = new List<AssetDepreciation>();

        foreach (var asset in assets)
        {
            // 计算已折旧月数
            var monthsUsed = ((year - asset.AcquisitionDate.Year) * 12 + month - asset.AcquisitionDate.Month);
            if (monthsUsed <= 0) continue;
            if (monthsUsed > asset.UsefulLifeMonths) continue;

            decimal depreciation = asset.DepreciationMethod switch
            {
                1 => CalculateStraightLine(asset, monthsUsed),
                2 => CalculateDoubleDeclining(asset, monthsUsed),
                3 => CalculateSumOfYears(asset, monthsUsed),
                _ => CalculateStraightLine(asset, monthsUsed)
            };

            if (depreciation <= 0) continue;

            var newAccumulated = asset.AccumulatedDepreciation + depreciation;
            var maxDepreciation = asset.OriginalValue - asset.ResidualValue;
            if (newAccumulated > maxDepreciation) depreciation = maxDepreciation - asset.AccumulatedDepreciation;
            if (depreciation <= 0) continue;

            newAccumulated = asset.AccumulatedDepreciation + depreciation;
            result.Add(new AssetDepreciation
            {
                AssetCardId = asset.Id, PeriodId = period.Id, Month = month,
                DepreciationAmount = depreciation, AccumulatedDepreciation = newAccumulated,
                NetValue = asset.OriginalValue - newAccumulated
            });
        }
        return result;
    }

    public async Task ConfirmDepreciationAsync(int year, int month, long currentUserId)
    {
        var period = await _db.Queryable<FinanceSystem.Modules.Accounts.Entities.AccountingPeriod>()
            .FirstAsync(p => p.PeriodYear == year && p.PeriodMonth == month);
        if (period == null) throw new NotFoundException("会计期间不存在");

        var hasExisting = await _db.Queryable<AssetDepreciation>().AnyAsync(d => d.PeriodId == period.Id);
        if (hasExisting) throw new BusinessException("当月折旧已确认");

        var items = await CalculateAsync(year, month);
        if (!items.Any()) throw new BusinessException("当月无可折旧资产");

        await _db.Insertable(items).ExecuteCommandAsync();

        // 更新资产卡片的累计折旧和净值
        foreach (var item in items)
        {
            var card = await _db.Queryable<AssetCard>().FirstAsync(a => a.Id == item.AssetCardId);
            if (card != null)
            {
                card.AccumulatedDepreciation = item.AccumulatedDepreciation;
                card.NetValue = item.NetValue;
                await _db.Updateable(card).ExecuteCommandAsync();
            }
        }

        // 自动生成折旧凭证：借记管理费用/制造费用，贷记累计折旧
        var depreciationPeriod = await _db.Queryable<AccountingPeriod>()
            .Where(p => p.BeginDate <= DateTime.Now && p.EndDate >= DateTime.Now && p.IsClosed == 0)
            .FirstAsync();

        if (depreciationPeriod != null)
        {
            var lastVoucher = await _db.Queryable<Voucher>()
                .Where(v => v.PeriodId == depreciationPeriod.Id)
                .OrderByDescending(v => v.Id)
                .FirstAsync();
            var nextNo = lastVoucher != null
                ? (int.Parse(lastVoucher.VoucherNo.Split('-').Last()) + 1).ToString("D4")
                : "0001";

            var entries = new List<VoucherEntry>();
            decimal totalDebit = 0;

            foreach (var item in items.Where(i => i.DepreciationAmount > 0))
            {
                var card = await _db.Queryable<AssetCard>().FirstAsync(a => a.Id == item.AssetCardId);
                if (card == null) continue;

                // 折旧费用科目：根据资产类别确定（默认管理费用-折旧费 6602）
                var expenseSubject = await _db.Queryable<AccountSubject>()
                    .FirstAsync(s => s.SubjectCode == "6602")
                    ?? await _db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode.StartsWith("660"));

                if (expenseSubject != null)
                {
                    entries.Add(new VoucherEntry
                    {
                        VoucherId = 0,
                        SubjectId = expenseSubject.Id,
                        Summary = $"{card.AssetName}折旧",
                        DebitAmount = item.DepreciationAmount,
                        CreditAmount = 0
                    });
                    totalDebit += item.DepreciationAmount;
                }
            }

            // 累计折旧科目（1602）
            var accumSubject = await _db.Queryable<AccountSubject>()
                .FirstAsync(s => s.SubjectCode == "1602");

            if (accumSubject != null && entries.Any())
            {
                entries.Add(new VoucherEntry
                {
                    VoucherId = 0,
                    SubjectId = accumSubject.Id,
                    Summary = "固定资产折旧",
                    DebitAmount = 0,
                    CreditAmount = totalDebit
                });

                var voucher = new Voucher
                {
                    VoucherNo = $"PZ-{depreciationPeriod.PeriodYear}{depreciationPeriod.PeriodMonth:D2}-{nextNo}",
                    VoucherDate = DateTime.Now,
                    PeriodId = depreciationPeriod.Id,
                    VoucherType = 2,
                    AbstractText = "固定资产折旧",
                    Status = 1,
                    TotalDebit = totalDebit,
                    TotalCredit = totalDebit,
                    PreparedBy = currentUserId,
                    ReviewedBy = currentUserId,
                    ReviewedTime = DateTime.Now,
                    Entries = entries
                };

                await _db.Insertable(voucher).ExecuteCommandAsync();

                // 回填凭证ID到折旧记录
                // 注意：AssetDepreciation实体可能没有VoucherId字段，需确认
                // 如果没有该字段则跳过回填
            }
        }
    }

    public async Task<List<object>> GetSummaryAsync(int year)
    {
        var list = await _db.Queryable<AssetCard>()
            .Where(a => a.Status != 5)
            .ToListAsync();

        return list.GroupBy(a => a.CategoryId).Select(g => (object)new
        {
            CategoryId = g.Key,
            Count = g.Count(),
            TotalOriginal = g.Sum(a => a.OriginalValue),
            TotalDepreciation = g.Sum(a => a.AccumulatedDepreciation),
            TotalNet = g.Sum(a => a.NetValue)
        }).ToList();
    }

    /// <summary>直线法折旧</summary>
    private static decimal CalculateStraightLine(AssetCard asset, int monthsUsed)
    {
        var depreciable = asset.OriginalValue - asset.ResidualValue;
        var monthly = Math.Round(depreciable / asset.UsefulLifeMonths, 2);
        var remaining = depreciable - asset.AccumulatedDepreciation;
        return Math.Min(monthly, remaining);
    }

    /// <summary>双倍余额递减法</summary>
    private static decimal CalculateDoubleDeclining(AssetCard asset, int monthsUsed)
    {
        var rate = 2.0m / asset.UsefulLifeMonths;
        var bookValue = asset.OriginalValue - asset.AccumulatedDepreciation;
        var depreciation = Math.Round(bookValue * rate, 2);
        var remaining = asset.OriginalValue - asset.ResidualValue - asset.AccumulatedDepreciation;
        return Math.Min(depreciation, remaining);
    }

    /// <summary>年数总和法</summary>
    private static decimal CalculateSumOfYears(AssetCard asset, int monthsUsed)
    {
        var depreciable = asset.OriginalValue - asset.ResidualValue;
        var remainingMonths = asset.UsefulLifeMonths - monthsUsed + 1;
        var totalMonths = asset.UsefulLifeMonths * (asset.UsefulLifeMonths + 1) / 2m;
        var depreciation = Math.Round(depreciable * remainingMonths / totalMonths, 2);
        var remaining = depreciable - asset.AccumulatedDepreciation;
        return Math.Min(depreciation, remaining);
    }
}

/// <summary>资产盘点服务实现</summary>
public class AssetInventoryService : IAssetInventoryService
{
    private readonly ISqlSugarClient _db;
    public AssetInventoryService(ISqlSugarClient db) => _db = db;

    public async Task<long> CreateAsync(AssetInventoryRequest request)
    {
        var entity = new AssetInventory
        {
            InventoryNo = request.InventoryNo,
            InventoryDate = request.InventoryDate,
            OperatorId = request.OperatorId,
            ItemsJson = System.Text.Json.JsonSerializer.Serialize(request.Items)
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<PageResult<AssetInventory>> GetListAsync(PageRequest query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<AssetInventory>()
            .OrderBy(i => i.InventoryDate, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<AssetInventory>(total, list);
    }

    public async Task CompleteAsync(long id)
    {
        var entity = await _db.Queryable<AssetInventory>().FirstAsync(i => i.Id == id) ?? throw new NotFoundException("盘点单不存在");
        entity.Status = 1;
        await _db.Updateable(entity).UpdateColumns(i => i.Status).ExecuteCommandAsync();
    }
}

/// <summary>资产报表服务实现</summary>
public class AssetReportService : IAssetReportService
{
    private readonly ISqlSugarClient _db;
    public AssetReportService(ISqlSugarClient db) => _db = db;

    public async Task<PageResult<object>> GetLedgerAsync(AssetReportQuery query)
    {
        RefAsync<int> total = 0;
        var q = _db.Queryable<AssetCard>()
            .WhereIF(!string.IsNullOrEmpty(query.AssetCode), c => c.AssetCode.Contains(query.AssetCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetName), c => c.AssetName.Contains(query.AssetName!))
            .WhereIF(query.CategoryId.HasValue, c => c.CategoryId == query.CategoryId)
            .WhereIF(query.Status.HasValue, c => c.Status == query.Status);
        var list = await q.Select(c => new { c.Id, c.AssetCode, c.AssetName, c.CategoryId, c.OriginalValue, c.AccumulatedDepreciation, c.NetValue, c.Status, c.Location })
            .OrderBy(c => c.AssetCode)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<object>(total, list.Cast<object>().ToList());
    }

    public async Task<List<object>> GetDepreciationSummaryAsync(int year)
    {
        var cards = await _db.Queryable<AssetCard>().Where(c => c.Status != 5).ToListAsync();
        return cards.Select(c => new
        {
            assetCode = c.AssetCode,
            assetName = c.AssetName,
            originalValue = c.OriginalValue,
            accumulatedDepreciation = c.AccumulatedDepreciation,
            netValue = c.NetValue,
            rate = c.OriginalValue > 0 ? Math.Round(c.AccumulatedDepreciation / c.OriginalValue * 100, 2) : 0m
        }).Cast<object>().ToList();
    }

    public async Task<object> GetValueStatsAsync()
    {
        var cards = await _db.Queryable<AssetCard>().ToListAsync();
        var totalOriginal = cards.Sum(c => c.OriginalValue);
        var totalDepreciation = cards.Sum(c => c.AccumulatedDepreciation);
        var totalNet = cards.Sum(c => c.NetValue);
        var inUse = cards.Count(c => c.Status == 1);
        var idle = cards.Count(c => c.Status == 2);
        return new { totalOriginal, totalDepreciation, totalNet, totalCount = cards.Count, inUse, idle };
    }
}
