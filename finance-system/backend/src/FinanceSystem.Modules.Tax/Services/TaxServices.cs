using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Modules.Tax.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Tax.Services;

/// <summary>税种服务接口</summary>
public interface ITaxCategoryService { Task<List<TaxCategory>> GetListAsync(); Task<long> CreateAsync(TaxCategoryRequest request); Task UpdateAsync(long id, TaxCategoryRequest request); Task DeleteAsync(long id); }
/// <summary>纳税申报服务接口</summary>
public interface ITaxDeclarationService { Task<PageResult<TaxDeclaration>> GetListAsync(TaxDeclarationQuery query); Task<long> CalculateAsync(TaxCalculateRequest request, long currentUserId); Task DeclareAsync(long id, long currentUserId); Task ConfirmPayAsync(long id); Task<List<TaxDeclaration>> CalculateSurchargesAsync(string declarePeriod, long vatDeclarationId, long currentUserId); }
/// <summary>发票服务接口</summary>
public interface ITaxInvoiceService { Task<PageResult<TaxInvoice>> GetListAsync(TaxInvoiceQuery query); Task<long> CreateAsync(TaxInvoiceRequest request); Task DeleteAsync(long id); Task VerifyAsync(long id); }

/// <summary>税务报表服务接口</summary>
public interface ITaxReportService
{
    /// <summary>获取税务汇总报表</summary>
    Task<object> GetSummaryAsync(int year);
    /// <summary>获取分税种申报汇总</summary>
    Task<List<object>> GetByCategoryAsync(int year, int? month);
    /// <summary>税负率分析（增值税税负率 + 综合税负率）</summary>
    Task<object> GetTaxBurdenAsync(int year, int? quarter);
}

/// <summary>税务日历服务接口</summary>
public interface ITaxCalendarService
{
    /// <summary>获取指定月份的税务日历事项</summary>
    Task<List<object>> GetCalendarAsync(int year, int month);
}

/// <summary>税种服务实现</summary>
public class TaxCategoryService : ITaxCategoryService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 税种管理服务
    /// </summary>
    public TaxCategoryService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<List<TaxCategory>> GetListAsync()
        => await _db.Queryable<TaxCategory>().Where(c => c.IsEnabled == 1).OrderBy(c => c.TaxCode).ToListAsync();

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(TaxCategoryRequest request)
    {
        var entity = new TaxCategory
        {
            TaxCode = request.TaxCode, TaxName = request.TaxName, TaxRate = request.TaxRate,
            CalculationMethod = request.CalculationMethod, DeclareCycle = request.DeclareCycle,
            SubjectId = request.SubjectId, Remark = request.Remark
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// <summary>
    /// 修改
    /// </summary>
    public async Task UpdateAsync(long id, TaxCategoryRequest request)
    {
        var entity = await _db.Queryable<TaxCategory>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("税种不存在");
        entity.TaxName = request.TaxName; entity.TaxRate = request.TaxRate;
        entity.SubjectId = request.SubjectId; entity.Remark = request.Remark;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 删除
    /// </summary>
    public async Task DeleteAsync(long id) => await _db.Deleteable<TaxCategory>().Where(c => c.Id == id).ExecuteCommandAsync();
}

/// <summary>纳税申报服务实现</summary>
public class TaxDeclarationService : ITaxDeclarationService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 纳税申报服务
    /// </summary>
    public TaxDeclarationService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<PageResult<TaxDeclaration>> GetListAsync(TaxDeclarationQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<TaxDeclaration>()
            .WhereIF(query.TaxCategoryId.HasValue, d => d.TaxCategoryId == query.TaxCategoryId)
            .WhereIF(!string.IsNullOrEmpty(query.DeclarePeriod), d => d.DeclarePeriod == query.DeclarePeriod)
            .WhereIF(query.Status.HasValue, d => d.Status == query.Status)
            .OrderBy(d => d.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<TaxDeclaration>(total, list);
    }

    /// <summary>
    /// <summary>
    /// 计算折旧
    /// </summary>
    public async Task<long> CalculateAsync(TaxCalculateRequest request, long currentUserId)
    {
        var tax = await _db.Queryable<TaxCategory>().FirstAsync(t => t.Id == request.TaxCategoryId)
            ?? throw new NotFoundException("税种不存在");

        // 根据税种类型从账务模块获取应纳税额基数
        decimal taxAmount = 0;
        if (tax.SubjectId.HasValue)
        {
            // 解析申报期间获取对应会计期间
            var periodParts = request.DeclarePeriod.Split('-');
            if (periodParts.Length == 2)
            {
                var periodYear = int.Parse(periodParts[0]);
                var periodMonth = int.Parse(periodParts[1]);
                var period = await _db.Queryable<AccountingPeriod>()
                    .FirstAsync(p => p.PeriodYear == periodYear && p.PeriodMonth == periodMonth);
                if (period != null)
                {
                    // 查询该科目在对应期间的余额（期末余额或本期发生额）
                    var balance = await _db.Queryable<SubjectBalance>()
                        .FirstAsync(b => b.SubjectId == tax.SubjectId.Value && b.PeriodId == period.Id);
                    if (balance != null)
                    {
                        // 应纳税额基数 = 根据余额方向取期末余额
                        decimal taxBase = balance.EndCredit - balance.EndDebit;
                        if (taxBase < 0) taxBase = balance.EndDebit - balance.EndCredit;
                        // 计算税额
                        taxAmount = Math.Round(taxBase * (tax.TaxRate / 100), 2);
                    }
                }
            }
        }
        else
        {
            // 无关联科目时使用请求中传入的税基
            taxAmount = Math.Round((request.TaxBase ?? 0) * (tax.TaxRate / 100), 2);
        }
        var hasExisting = await _db.Queryable<TaxDeclaration>()
            .AnyAsync(d => d.TaxCategoryId == request.TaxCategoryId && d.DeclarePeriod == request.DeclarePeriod);
        if (hasExisting) throw new BusinessException("该期间已存在申报记录");

        var entity = new TaxDeclaration
        {
            TaxCategoryId = request.TaxCategoryId, DeclarePeriod = request.DeclarePeriod,
            TaxAmount = taxAmount, ActualPaidAmount = 0, Status = 0, DeclaredBy = currentUserId
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// <summary>
    /// 提交申报
    /// </summary>
    public async Task DeclareAsync(long id, long currentUserId)
    {
        var entity = await _db.Queryable<TaxDeclaration>().FirstAsync(d => d.Id == id) ?? throw new NotFoundException("申报记录不存在");
        if (entity.Status != 0) throw new BusinessException("非待申报状态");
        entity.Status = 1; entity.DeclaredBy = currentUserId;
        await _db.Updateable(entity).UpdateColumns(d => new { d.Status, d.DeclaredBy }).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 确认缴款
    /// </summary>
    public async Task ConfirmPayAsync(long id)
    {
        var entity = await _db.Queryable<TaxDeclaration>().FirstAsync(d => d.Id == id) ?? throw new NotFoundException("申报记录不存在");
        if (entity.Status != 1) throw new BusinessException("非已申报状态");
        entity.Status = 2; entity.ActualPaidAmount = entity.TaxAmount;
        await _db.Updateable(entity).UpdateColumns(d => new { d.Status, d.ActualPaidAmount }).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据增值税申报自动计算附加税（城建税7%+教育费附加3%+地方教育附加2%）
    /// </summary>
    public async Task<List<TaxDeclaration>> CalculateSurchargesAsync(string declarePeriod, long vatDeclarationId, long currentUserId)
    {
        var vatDecl = await _db.Queryable<TaxDeclaration>().FirstAsync(d => d.Id == vatDeclarationId)
            ?? throw new NotFoundException("增值税申报记录不存在");

        // 附加税税率：城建税7%（市区）/5%（县城） + 教育费附加3% + 地方教育附加2%
        var surcharges = new[]
        {
            new { Name = "城市维护建设税", Code = "CJCS", Rate = 7m },
            new { Name = "教育费附加", Code = "JYFJ", Rate = 3m },
            new { Name = "地方教育附加", Code = "DFJYFJ", Rate = 2m }
        };

        var results = new List<TaxDeclaration>();
        foreach (var sc in surcharges)
        {
            var taxCategory = await _db.Queryable<TaxCategory>().FirstAsync(t => t.TaxCode == sc.Code);
            if (taxCategory == null)
            {
                // 自动创建附加税种
                taxCategory = new TaxCategory
                {
                    TaxCode = sc.Code, TaxName = sc.Name, TaxRate = sc.Rate,
                    CalculationMethod = 1, DeclareCycle = 1
                };
                await _db.Insertable(taxCategory).ExecuteCommandAsync();
            }

            decimal surchargeAmount = Math.Round(vatDecl.TaxAmount * sc.Rate / 100, 2);

            var hasExisting = await _db.Queryable<TaxDeclaration>()
                .AnyAsync(d => d.TaxCategoryId == taxCategory.Id && d.DeclarePeriod == declarePeriod);
            if (hasExisting) continue;

            var entity = new TaxDeclaration
            {
                TaxCategoryId = taxCategory.Id, DeclarePeriod = declarePeriod,
                TaxAmount = surchargeAmount, ActualPaidAmount = 0, Status = 0, DeclaredBy = currentUserId
            };
            await _db.Insertable(entity).ExecuteCommandAsync();
            results.Add(entity);
        }
        return results;
    }
}

/// <summary>发票服务实现</summary>
public class TaxInvoiceService : ITaxInvoiceService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 发票管理服务
    /// </summary>
    public TaxInvoiceService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<PageResult<TaxInvoice>> GetListAsync(TaxInvoiceQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<TaxInvoice>()
            .WhereIF(query.Direction.HasValue, i => i.Direction == query.Direction)
            .WhereIF(query.InvoiceType.HasValue, i => i.InvoiceType == query.InvoiceType)
            .WhereIF(!string.IsNullOrEmpty(query.InvoiceNo), i => i.InvoiceNo.Contains(query.InvoiceNo!))
            .WhereIF(query.IsVerified.HasValue, i => i.IsVerified == query.IsVerified)
            .OrderBy(i => i.InvoiceDate, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<TaxInvoice>(total, list);
    }

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(TaxInvoiceRequest request)
    {
        var entity = new TaxInvoice
        {
            InvoiceType = request.InvoiceType, InvoiceNo = request.InvoiceNo,
            InvoiceDate = request.InvoiceDate, CounterpartyName = request.CounterpartyName,
            TaxAmount = request.TaxAmount, AmountWithoutTax = request.AmountWithoutTax,
            TotalAmount = request.AmountWithoutTax + request.TaxAmount,
            Direction = request.Direction, IsVerified = 0, Remark = request.Remark
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 删除发票
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Queryable<TaxInvoice>().FirstAsync(i => i.Id == id)
            ?? throw new NotFoundException("发票不存在");
        if (entity.IsVerified == 1)
            throw new InvalidOperationException("已验真的发票不可删除");
        await _db.Deleteable<TaxInvoice>().Where(i => i.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// <inheritdoc/>
    public async Task VerifyAsync(long id)
    {
        var entity = await _db.Queryable<TaxInvoice>().FirstAsync(i => i.Id == id) ?? throw new NotFoundException("发票不存在");
        entity.IsVerified = 1;
        await _db.Updateable(entity).UpdateColumns(i => i.IsVerified).ExecuteCommandAsync();
    }
}

/// <summary>税务报表服务实现</summary>
public class TaxReportService : ITaxReportService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 税务报表服务
    /// </summary>
    public TaxReportService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取汇总
    /// </summary>
    public async Task<object> GetSummaryAsync(int year)
    {
        var declarations = await _db.Queryable<TaxDeclaration>()
            .Where(d => d.DeclarePeriod.StartsWith(year.ToString()))
            .ToListAsync();
        var totalTaxAmount = declarations.Sum(d => d.TaxAmount);
        var totalPaid = declarations.Sum(d => d.ActualPaidAmount);
        var unpaid = declarations.Where(d => d.Status == 1).Sum(d => d.TaxAmount);
        var categories = await _db.Queryable<TaxCategory>().Where(c => c.IsEnabled == 1).ToListAsync();
        var byCategory = categories.Select(c => new
        {
            taxName = c.TaxName,
            taxAmount = declarations.Where(d => d.TaxCategoryId == c.Id).Sum(d => d.TaxAmount),
            paidAmount = declarations.Where(d => d.TaxCategoryId == c.Id).Sum(d => d.ActualPaidAmount)
        }).ToList();
        return new { year, totalTaxAmount, totalPaid, unpaid, byCategory };
    }

    /// <summary>
    /// <summary>
    /// 按税种分类查询
    /// </summary>
    public async Task<List<object>> GetByCategoryAsync(int year, int? month)
    {
        var query = _db.Queryable<TaxDeclaration>().Where(d => d.DeclarePeriod.StartsWith(year.ToString()));
        if (month.HasValue) query = query.Where(d => d.DeclarePeriod == $"{year}-{month.Value:D2}");
        var declarations = await query.ToListAsync();
        var categories = await _db.Queryable<TaxCategory>().Where(c => c.IsEnabled == 1).ToListAsync();
        return categories.Select(c =>
        {
            var related = declarations.Where(d => d.TaxCategoryId == c.Id).ToList();
            return (object)new
            {
                categoryId = c.Id, taxName = c.TaxName, taxCode = c.TaxCode,
                totalAmount = related.Sum(d => d.TaxAmount),
                paidAmount = related.Sum(d => d.ActualPaidAmount),
                count = related.Count
            };
        }).ToList();
    }

    /// <summary>税负率分析</summary>
    public async Task<object> GetTaxBurdenAsync(int year, int? quarter)
    {
        var query = _db.Queryable<TaxDeclaration>()
            .Where(d => d.DeclarePeriod.StartsWith(year.ToString()) && d.Status >= 1);

        List<TaxDeclaration> declarations;
        if (quarter.HasValue)
        {
            var qv = quarter.Value;
            var startMonth = (qv - 1) * 3 + 1;
            var endMonth = qv * 3;
            var allDecls = await query.ToListAsync();
            declarations = allDecls.Where(d =>
            {
                var parts = d.DeclarePeriod.Split('-');
                return parts.Length == 2 && int.TryParse(parts[1], out var m)
                    && m >= startMonth && m <= endMonth;
            }).ToList();
        }
        else
        {
            declarations = await query.ToListAsync();
        }

        var totalTaxPaid = declarations.Sum(d => d.ActualPaidAmount);

        var periodIds = await _db.Queryable<AccountingPeriod>()
            .Where(p => p.PeriodYear == year).Select(p => p.Id).ToListAsync();
        var revenueSubjectIds = await _db.Queryable<AccountSubject>()
            .Where(s => s.SubjectCode.StartsWith("6001") || s.SubjectCode.StartsWith("6051"))
            .Select(s => s.Id).ToListAsync();

        decimal revenue = 0;
        if (periodIds.Any() && revenueSubjectIds.Any())
        {
            revenue = (await _db.Queryable<VoucherEntry>()
                .LeftJoin<Voucher>((e, v) => e.VoucherId == v.Id)
                .Where((e, v) => periodIds.Contains(v.PeriodId) && v.Status == 1 && revenueSubjectIds.Contains(e.SubjectId))
                .Select((e, v) => e.CreditAmount).ToListAsync()).Sum();
        }

        var vatCategory = await _db.Queryable<TaxCategory>().FirstAsync(t => t.TaxName.Contains("增值税"));
        decimal vatPaid = 0;
        if (vatCategory != null)
            vatPaid = declarations.Where(d => d.TaxCategoryId == vatCategory.Id).Sum(d => d.ActualPaidAmount);

        return new
        {
            Year = year, Quarter = quarter,
            TotalRevenue = revenue, TotalTaxPaid = totalTaxPaid, VatPaid = vatPaid,
            VatBurdenRate = revenue > 0 ? Math.Round(vatPaid / revenue * 100, 2) : 0,
            TotalBurdenRate = revenue > 0 ? Math.Round(totalTaxPaid / revenue * 100, 2) : 0
        };
    }
}

/// <summary>税务日历服务实现</summary>
public class TaxCalendarService : ITaxCalendarService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <inheritdoc/>
    public TaxCalendarService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <inheritdoc/>
    public async Task<List<object>> GetCalendarAsync(int year, int month)
    {
        var categories = await _db.Queryable<TaxCategory>().Where(c => c.IsEnabled == 1).ToListAsync();
        var period = $"{year}-{month:D2}";
        var existingDeclarations = await _db.Queryable<TaxDeclaration>()
            .Where(d => d.DeclarePeriod == period)
            .ToListAsync();
        return categories.Select(c =>
        {
            var existing = existingDeclarations.FirstOrDefault(d => d.TaxCategoryId == c.Id);
            int deadline = c.DeclareCycle switch { 1 => 15, 3 => month % 3 == 0 ? 15 : 0, _ => 15 };
            if (deadline <= 0) return null;
            return (object)new
            {
                categoryId = c.Id, taxName = c.TaxName, taxCode = c.TaxCode,
                deadline = new DateTime(year, month, deadline),
                status = existing?.Status ?? -1,
                taxAmount = existing?.TaxAmount ?? 0,
                paidAmount = existing?.ActualPaidAmount ?? 0
            };
        }).Where(x => x != null).Cast<object>().ToList();
    }
}
