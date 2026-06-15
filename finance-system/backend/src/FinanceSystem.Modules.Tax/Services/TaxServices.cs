using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Modules.Tax.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Tax.Services;

/// <summary>税种服务接口</summary>
public interface ITaxCategoryService { Task<List<TaxCategory>> GetListAsync(); Task<long> CreateAsync(TaxCategoryRequest request); Task UpdateAsync(long id, TaxCategoryRequest request); Task DeleteAsync(long id); }
/// <summary>纳税申报服务接口</summary>
public interface ITaxDeclarationService { Task<PageResult<TaxDeclaration>> GetListAsync(TaxDeclarationQuery query); Task<long> CalculateAsync(TaxCalculateRequest request); Task DeclareAsync(long id, long currentUserId); Task ConfirmPayAsync(long id); }
/// <summary>发票服务接口</summary>
public interface ITaxInvoiceService { Task<PageResult<TaxInvoice>> GetListAsync(TaxInvoiceQuery query); Task<long> CreateAsync(TaxInvoiceRequest request); Task VerifyAsync(long id); }

/// <summary>税种服务实现</summary>
public class TaxCategoryService : ITaxCategoryService
{
    private readonly ISqlSugarClient _db;
    public TaxCategoryService(ISqlSugarClient db) => _db = db;

    public async Task<List<TaxCategory>> GetListAsync()
        => await _db.Queryable<TaxCategory>().Where(c => c.IsEnabled == 1).OrderBy(c => c.TaxCode).ToListAsync();

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

    public async Task UpdateAsync(long id, TaxCategoryRequest request)
    {
        var entity = await _db.Queryable<TaxCategory>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("税种不存在");
        entity.TaxName = request.TaxName; entity.TaxRate = request.TaxRate;
        entity.SubjectId = request.SubjectId; entity.Remark = request.Remark;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id) => await _db.Deleteable<TaxCategory>().Where(c => c.Id == id).ExecuteCommandAsync();
}

/// <summary>纳税申报服务实现</summary>
public class TaxDeclarationService : ITaxDeclarationService
{
    private readonly ISqlSugarClient _db;
    public TaxDeclarationService(ISqlSugarClient db) => _db = db;

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

    public async Task<long> CalculateAsync(TaxCalculateRequest request)
    {
        var tax = await _db.Queryable<TaxCategory>().FirstAsync(t => t.Id == request.TaxCategoryId)
            ?? throw new NotFoundException("税种不存在");

        // TODO: 根据税种类型从账务模块获取应纳税额基数
        decimal taxAmount = 0;
        var hasExisting = await _db.Queryable<TaxDeclaration>()
            .AnyAsync(d => d.TaxCategoryId == request.TaxCategoryId && d.DeclarePeriod == request.DeclarePeriod);
        if (hasExisting) throw new BusinessException("该期间已存在申报记录");

        var entity = new TaxDeclaration
        {
            TaxCategoryId = request.TaxCategoryId, DeclarePeriod = request.DeclarePeriod,
            TaxAmount = taxAmount, ActualPaidAmount = 0, Status = 0, DeclaredBy = 0
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task DeclareAsync(long id, long currentUserId)
    {
        var entity = await _db.Queryable<TaxDeclaration>().FirstAsync(d => d.Id == id) ?? throw new NotFoundException("申报记录不存在");
        if (entity.Status != 0) throw new BusinessException("非待申报状态");
        entity.Status = 1; entity.DeclaredBy = currentUserId;
        await _db.Updateable(entity).UpdateColumns(d => new { d.Status, d.DeclaredBy }).ExecuteCommandAsync();
    }

    public async Task ConfirmPayAsync(long id)
    {
        var entity = await _db.Queryable<TaxDeclaration>().FirstAsync(d => d.Id == id) ?? throw new NotFoundException("申报记录不存在");
        if (entity.Status != 1) throw new BusinessException("非已申报状态");
        entity.Status = 2; entity.ActualPaidAmount = entity.TaxAmount;
        await _db.Updateable(entity).UpdateColumns(d => new { d.Status, d.ActualPaidAmount }).ExecuteCommandAsync();
    }
}

/// <summary>发票服务实现</summary>
public class TaxInvoiceService : ITaxInvoiceService
{
    private readonly ISqlSugarClient _db;
    public TaxInvoiceService(ISqlSugarClient db) => _db = db;

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

    public async Task VerifyAsync(long id)
    {
        var entity = await _db.Queryable<TaxInvoice>().FirstAsync(i => i.Id == id) ?? throw new NotFoundException("发票不存在");
        entity.IsVerified = 1;
        await _db.Updateable(entity).UpdateColumns(i => i.IsVerified).ExecuteCommandAsync();
    }
}
