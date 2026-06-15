using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Expense.DTOs;
using FinanceSystem.Modules.Expense.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Expense.Services;

/// <summary>费用类型服务接口</summary>
public interface IExpenseTypeService { Task<List<ExpenseType>> GetListAsync(); Task<long> CreateAsync(ExpenseTypeRequest request); Task UpdateAsync(long id, ExpenseTypeRequest request); Task DeleteAsync(long id); }
/// <summary>费用报销服务接口</summary>
public interface IExpenseClaimService { Task<PageResult<ExpenseClaim>> GetListAsync(ExpenseClaimQuery query); Task<ExpenseClaim?> GetByIdAsync(long id); Task<long> CreateAsync(ExpenseClaimRequest request, long currentUserId); Task SubmitAsync(long id, long currentUserId); Task ApproveAsync(long id); Task RejectAsync(long id); Task ConfirmPaymentAsync(long id); }
/// <summary>费用统计服务接口</summary>
public interface IExpenseStatisticsService { Task<List<object>> GetStatisticsAsync(ExpenseStatisticsQuery query); }

/// <summary>费用类型服务实现</summary>
public class ExpenseTypeService : IExpenseTypeService
{
    private readonly ISqlSugarClient _db;
    public ExpenseTypeService(ISqlSugarClient db) => _db = db;

    public async Task<List<ExpenseType>> GetListAsync()
        => await _db.Queryable<ExpenseType>().Where(t => t.IsEnabled == 1).OrderBy(t => t.SortOrder).ToListAsync();

    public async Task<long> CreateAsync(ExpenseTypeRequest request)
    {
        var entity = new ExpenseType
        {
            TypeCode = request.TypeCode, TypeName = request.TypeName, SubjectId = request.SubjectId,
            SingleLimit = request.SingleLimit, MonthlyLimit = request.MonthlyLimit, SortOrder = request.SortOrder
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(long id, ExpenseTypeRequest request)
    {
        var entity = await _db.Queryable<ExpenseType>().FirstAsync(t => t.Id == id) ?? throw new NotFoundException("费用类型不存在");
        entity.TypeName = request.TypeName; entity.SubjectId = request.SubjectId;
        entity.SingleLimit = request.SingleLimit; entity.MonthlyLimit = request.MonthlyLimit;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id) => await _db.Deleteable<ExpenseType>().Where(t => t.Id == id).ExecuteCommandAsync();
}

/// <summary>费用报销服务实现</summary>
public class ExpenseClaimService : IExpenseClaimService
{
    private readonly ISqlSugarClient _db;
    public ExpenseClaimService(ISqlSugarClient db) => _db = db;

    public async Task<PageResult<ExpenseClaim>> GetListAsync(ExpenseClaimQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<ExpenseClaim>()
            .WhereIF(query.Status.HasValue, c => c.Status == query.Status)
            .WhereIF(query.ClaimantId.HasValue, c => c.ClaimantId == query.ClaimantId)
            .WhereIF(query.DeptId.HasValue, c => c.DeptId == query.DeptId)
            .OrderBy(c => c.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<ExpenseClaim>(total, list);
    }

    public async Task<ExpenseClaim?> GetByIdAsync(long id)
    {
        return await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id);
    }

    public async Task<long> CreateAsync(ExpenseClaimRequest request, long currentUserId)
    {
        if (request.Items == null || !request.Items.Any()) throw new BusinessException("报销单至少包含一条费用明细");

        var ym = DateTime.Now.ToString("yyyyMM");
        var count = await _db.Queryable<ExpenseClaim>().Where(c => c.ClaimNo.Contains(ym)).CountAsync() + 1;
        var claimNo = $"BX-{ym}-{count:D4}";
        var totalAmount = request.Items.Sum(i => i.Amount);

        var claim = new ExpenseClaim
        {
            ClaimNo = claimNo, Title = request.Title, ClaimantId = currentUserId,
            TotalAmount = totalAmount, Status = 0, Remark = request.Remark
        };
        await _db.Insertable(claim).ExecuteCommandAsync();

        var items = request.Items.Select(i => new ExpenseItem
        {
            ClaimId = claim.Id, ExpenseTypeId = i.ExpenseTypeId, Description = i.Description,
            Amount = i.Amount, ExpenseDate = i.ExpenseDate, InvoiceNo = i.InvoiceNo
        }).ToList();
        await _db.Insertable(items).ExecuteCommandAsync();

        return claim.Id;
    }

    public async Task SubmitAsync(long id, long currentUserId)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 0) throw new BusinessException("仅草稿状态可提交");
        claim.Status = 1;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    public async Task ApproveAsync(long id)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 1) throw new BusinessException("非审批中状态");
        claim.Status = 2;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    public async Task RejectAsync(long id)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 1) throw new BusinessException("非审批中状态");
        claim.Status = 3;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    public async Task ConfirmPaymentAsync(long id)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 2) throw new BusinessException("非已通过状态");
        claim.Status = 4; claim.PaymentDate = DateTime.Now;
        await _db.Updateable(claim).UpdateColumns(c => new { c.Status, c.PaymentDate }).ExecuteCommandAsync();
        // TODO: 自动生成费用凭证
    }
}

/// <summary>费用统计服务实现</summary>
public class ExpenseStatisticsService : IExpenseStatisticsService
{
    private readonly ISqlSugarClient _db;
    public ExpenseStatisticsService(ISqlSugarClient db) => _db = db;

    public async Task<List<object>> GetStatisticsAsync(ExpenseStatisticsQuery query)
    {
        var startDate = DateTime.Parse(query.StartDate);
        var endDate = DateTime.Parse(query.EndDate);

        var items = await _db.Queryable<ExpenseItem>()
            .LeftJoin<ExpenseClaim>((i, c) => i.ClaimId == c.Id)
            .Where((i, c) => c.Status >= 2 && i.ExpenseDate >= startDate && i.ExpenseDate <= endDate)
            .WhereIF(query.DeptId.HasValue, (i, c) => c.DeptId == query.DeptId)
            .WhereIF(query.ExpenseTypeId.HasValue, i => i.ExpenseTypeId == query.ExpenseTypeId)
            .Select((i, c) => new { i.ExpenseTypeId, i.Amount })
            .ToListAsync();

        return items.GroupBy(x => x.ExpenseTypeId).Select(g => (object)new
        {
            ExpenseTypeId = g.Key,
            TotalAmount = g.Sum(x => x.Amount),
            Count = g.Count()
        }).ToList();
    }
}
