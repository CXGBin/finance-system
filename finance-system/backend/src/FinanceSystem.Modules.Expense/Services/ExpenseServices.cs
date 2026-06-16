using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Expense.DTOs;
using FinanceSystem.Modules.Expense.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Expense.Services;

/// <summary>费用类型服务接口</summary>
public interface IExpenseTypeService { Task<List<ExpenseType>> GetListAsync(); Task<long> CreateAsync(ExpenseTypeRequest request); Task UpdateAsync(long id, ExpenseTypeRequest request); Task DeleteAsync(long id); }
/// <summary>费用报销服务接口</summary>
public interface IExpenseClaimService { Task<PageResult<ExpenseClaim>> GetListAsync(ExpenseClaimQuery query); Task<ExpenseClaim?> GetByIdAsync(long id); Task<long> CreateAsync(ExpenseClaimRequest request, long currentUserId); Task UpdateAsync(long id, ExpenseClaimRequest request); Task SubmitAsync(long id, long currentUserId); Task ApproveAsync(long id); Task RejectAsync(long id); Task ConfirmPaymentAsync(long id, long currentUserId); }
/// <summary>费用统计服务接口</summary>
public interface IExpenseStatisticsService { Task<List<object>> GetStatisticsAsync(ExpenseStatisticsQuery query); }

/// <summary>费用类型服务实现</summary>
public class ExpenseTypeService : IExpenseTypeService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 费用类型服务
    /// </summary>
    public ExpenseTypeService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<List<ExpenseType>> GetListAsync()
        => await _db.Queryable<ExpenseType>().Where(t => t.IsEnabled == 1).OrderBy(t => t.SortOrder).ToListAsync();

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
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

    /// <summary>
    /// <summary>
    /// 修改
    /// </summary>
    public async Task UpdateAsync(long id, ExpenseTypeRequest request)
    {
        var entity = await _db.Queryable<ExpenseType>().FirstAsync(t => t.Id == id) ?? throw new NotFoundException("费用类型不存在");
        entity.TypeName = request.TypeName; entity.SubjectId = request.SubjectId;
        entity.SingleLimit = request.SingleLimit; entity.MonthlyLimit = request.MonthlyLimit;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 删除
    /// </summary>
    public async Task DeleteAsync(long id) => await _db.Deleteable<ExpenseType>().Where(t => t.Id == id).ExecuteCommandAsync();
}

/// <summary>费用报销服务实现</summary>
public class ExpenseClaimService : IExpenseClaimService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 费用报销服务
    /// </summary>
    public ExpenseClaimService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
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

    /// <summary>
    /// <summary>
    /// 获取详情
    /// </summary>
    public async Task<ExpenseClaim?> GetByIdAsync(long id)
    {
        return await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id);
    }

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
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

    /// <summary>
    /// UpdateAsync方法
    /// </summary>
    public async Task UpdateAsync(long id, ExpenseClaimRequest request)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 0) throw new BusinessException("仅草稿状态可修改");
        claim.Title = request.Title;
        claim.Remark = request.Remark;
        claim.TotalAmount = request.Items.Sum(i => i.Amount);
        await _db.Updateable(claim).ExecuteCommandAsync();
        // 更新明细
        await _db.Deleteable<ExpenseItem>().Where(i => i.ClaimId == id).ExecuteCommandAsync();
        var newItems = request.Items.Select(i => new ExpenseItem
        {
            ClaimId = id,
            ExpenseDate = i.ExpenseDate,
            Description = i.Description,
            Amount = i.Amount,
            InvoiceNo = i.InvoiceNo,
            ExpenseTypeId = i.ExpenseTypeId
        }).ToList();
        await _db.Insertable(newItems).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 提交审批
    /// </summary>
    public async Task SubmitAsync(long id, long currentUserId)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 0) throw new BusinessException("仅草稿状态可提交");
        claim.Status = 1;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 审批通过
    /// </summary>
    public async Task ApproveAsync(long id)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 1) throw new BusinessException("非审批中状态");

        // 费用类型预算校验：检查是否超过月度限额
        var expenseItems = await _db.Queryable<ExpenseItem>().Where(i => i.ClaimId == claim.Id).ToListAsync();
        var typeIds = expenseItems.Select(i => i.ExpenseTypeId).Distinct().ToList();
        var types = await _db.Queryable<ExpenseType>().Where(t => typeIds.Contains(t.Id)).ToListAsync();

        var now = DateTime.Now;
        foreach (var type in types.Where(t => t.MonthlyLimit > 0))
        {
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthTotal = await _db.Queryable<ExpenseItem>()
                .LeftJoin<ExpenseClaim>((i, c) => i.ClaimId == c.Id)
                .Where((i, c) => i.ExpenseTypeId == type.Id && c.Status >= 2 && c.CreatedTime >= monthStart)
                .SumAsync((i, c) => i.Amount);
            var thisClaimAmount = expenseItems.Where(i => i.ExpenseTypeId == type.Id).Sum(i => i.Amount);
            if (monthTotal + thisClaimAmount > type.MonthlyLimit)
                throw new BusinessException($"费用类型[{type.TypeName}]本月累计报销{monthTotal + thisClaimAmount}元，超出月度限额{type.MonthlyLimit}元");
        }

        claim.Status = 2;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 审批驳回
    /// </summary>
    public async Task RejectAsync(long id)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 1) throw new BusinessException("非审批中状态");
        claim.Status = 3;
        await _db.Updateable(claim).UpdateColumns(c => c.Status).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 确认付款
    /// </summary>
    public async Task ConfirmPaymentAsync(long id, long currentUserId)
    {
        var claim = await _db.Queryable<ExpenseClaim>().FirstAsync(c => c.Id == id) ?? throw new NotFoundException("报销单不存在");
        if (claim.Status != 2) throw new BusinessException("非已通过状态");
        claim.Status = 4; claim.PaymentDate = DateTime.Now;
        await _db.Updateable(claim).UpdateColumns(c => new { c.Status, c.PaymentDate }).ExecuteCommandAsync();
        // 付款后自动生成费用凭证：借记管理费用等，贷记银行存款/库存现金
        var period = await _db.Queryable<AccountingPeriod>()
            .Where(p => p.BeginDate <= DateTime.Now && p.EndDate >= DateTime.Now && p.IsClosed == 0)
            .FirstAsync();
        if (period != null && claim.TotalAmount > 0)
        {
            var lastVoucher = await _db.Queryable<Voucher>()
                .Where(v => v.PeriodId == period.Id).OrderByDescending(v => v.Id).FirstAsync();
            var nextNo = lastVoucher != null
                ? (int.Parse(lastVoucher.VoucherNo.Split('-').Last()) + 1).ToString("D4") : "0001";

            // 查找费用报销关联的科目（从报销明细中获取费用类型关联的科目）
            var expenseItems = await _db.Queryable<ExpenseItem>().Where(i => i.ClaimId == claim.Id).ToListAsync();
            var expenseTypeIds = expenseItems.Select(i => i.ExpenseTypeId).Distinct().ToList();
            var expenseTypes = await _db.Queryable<ExpenseType>()
                .Where(t => expenseTypeIds.Contains(t.Id)).ToListAsync();

            var entries = new List<VoucherEntry>();
            decimal totalDebit = 0;

            // 按费用类型汇总，生成借方分录
            var groupedByType = expenseItems.GroupBy(i => i.ExpenseTypeId);
            foreach (var group in groupedByType)
            {
                var expType = expenseTypes.FirstOrDefault(t => t.Id == group.Key);
                // 确定费用科目：优先使用费用类型关联的科目，否则默认管理费用（6602）
                long subjectId;
                if (expType?.SubjectId.HasValue == true && expType.SubjectId.Value > 0)
                {
                    subjectId = expType.SubjectId.Value;
                }
                else
                {
                    var defaultSubject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "6602");
                    subjectId = defaultSubject?.Id ?? 0;
                }
                var amount = group.Sum(i => i.Amount);
                if (subjectId > 0 && amount > 0)
                {
                    entries.Add(new VoucherEntry
                    {
                        VoucherId = 0, SubjectId = subjectId,
                        Summary = $"{expType?.TypeName ?? "费用报销"}-报销单{claim.ClaimNo}",
                        DebitAmount = amount, CreditAmount = 0
                    });
                    totalDebit += amount;
                }
            }

            // 贷记银行存款（1002）
            var bankSubject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == "1002");
            if (bankSubject != null && entries.Any())
            {
                entries.Add(new VoucherEntry
                {
                    VoucherId = 0, SubjectId = bankSubject.Id,
                    Summary = $"报销付款-{claim.ClaimNo}",
                    DebitAmount = 0, CreditAmount = totalDebit
                });

                var voucher = new Voucher
                {
                    VoucherNo = $"PZ-{period.PeriodYear}{period.PeriodMonth:D2}-{nextNo}",
                    VoucherDate = DateTime.Now, PeriodId = period.Id, VoucherType = 1,
                    AbstractText = $"费用报销付款-{claim.Title}",
                    Status = 1, TotalDebit = totalDebit, TotalCredit = totalDebit,
                    PreparedBy = currentUserId, ReviewedBy = currentUserId, ReviewedTime = DateTime.Now,
                    Entries = entries
                };
                await _db.Insertable(voucher).ExecuteCommandAsync();
                claim.VoucherId = voucher.Id;
                await _db.Updateable(claim).UpdateColumns(c => c.VoucherId).ExecuteCommandAsync();
            }
        }
    }
}

/// <summary>费用统计服务实现</summary>
public class ExpenseStatisticsService : IExpenseStatisticsService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <inheritdoc/>
    public ExpenseStatisticsService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取统计
    /// </summary>
    public async Task<List<object>> GetStatisticsAsync(ExpenseStatisticsQuery query)
    {
        DateTime startDate, endDate;
        if (string.IsNullOrWhiteSpace(query.StartDate) || !DateTime.TryParse(query.StartDate, out startDate))
            startDate = new DateTime(query.Year ?? DateTime.Now.Year, 1, 1);
        if (string.IsNullOrWhiteSpace(query.EndDate) || !DateTime.TryParse(query.EndDate, out endDate))
            endDate = new DateTime(query.Year ?? DateTime.Now.Year, 12, 31, 23, 59, 59);

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

/// <summary>费用分摊服务接口</summary>
public interface IExpenseAllocateService
{
    /// <summary>创建分摊单</summary>
    Task<long> CreateAsync(ExpenseAllocateRequest request);
    /// <summary>查询分摊列表</summary>
    Task<PageResult<ExpenseAllocate>> GetListAsync(PageRequest query);
}

/// <summary>费用分摊服务实现</summary>
public class ExpenseAllocateService : IExpenseAllocateService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 费用分摊服务
    /// </summary>
    public ExpenseAllocateService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(ExpenseAllocateRequest request)
    {
        var entity = new ExpenseAllocate
        {
            AllocateNo = request.AllocateNo,
            Description = request.Description,
            TotalAmount = request.TotalAmount,
            DeptId = request.DeptId,
            AllocateAmount = request.AllocateAmount,
            PeriodYear = request.PeriodYear,
            PeriodMonth = request.PeriodMonth
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<PageResult<ExpenseAllocate>> GetListAsync(PageRequest query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<ExpenseAllocate>()
            .OrderBy(a => a.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<ExpenseAllocate>(total, list);
    }
}

/// <summary>借款服务接口</summary>
public interface IExpenseLoanService
{
    Task<PageResult<ExpenseLoan>> GetListAsync(ExpenseLoanQuery query);
    Task<ExpenseLoan?> GetByIdAsync(long id);
    Task<long> CreateAsync(ExpenseLoanRequest request, long currentUserId);
    Task ApproveAsync(long id, long currentUserId);
    Task RejectAsync(long id);
    /// <summary>核销借款（从报销单中抵扣）</summary>
    Task SettleAsync(long loanId, decimal amount, long claimId);
}

/// <summary>借款服务实现</summary>
public class ExpenseLoanService : IExpenseLoanService
{
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// <summary>
    /// 借款管理服务
    /// </summary>
    public ExpenseLoanService(ISqlSugarClient db) => _db = db;

    /// <summary>
    /// <summary>
    /// 获取盘点列表
    /// </summary>
    public async Task<PageResult<ExpenseLoan>> GetListAsync(ExpenseLoanQuery query)
    {
        var q = _db.Queryable<ExpenseLoan>();
        q = q.WhereIF(query.Status.HasValue, l => l.Status == query.Status);
        var keyword = query.Keyword ?? "";
        q = q.WhereIF(!string.IsNullOrEmpty(keyword), l => l.LoanNo.Contains(keyword) || (l.Reason ?? "").Contains(keyword));
        var total = await q.CountAsync();
        var list = await q.OrderByDescending(l => l.Id).ToPageListAsync(query.PageIndex, query.PageSize);
        return new PageResult<ExpenseLoan>(total, list);
    }

    /// <summary>
    /// <summary>
    /// 获取详情
    /// </summary>
    public async Task<ExpenseLoan?> GetByIdAsync(long id)
    {
        return await _db.Queryable<ExpenseLoan>().FirstAsync(l => l.Id == id);
    }

    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(ExpenseLoanRequest request, long currentUserId)
    {
        if (request.LoanAmount <= 0) throw new BusinessException("借款金额必须大于0");
        var count = await _db.Queryable<ExpenseLoan>().CountAsync();
        var loan = new ExpenseLoan
        {
            LoanNo = $"JK-{count + 1:D4}",
            ApplicantId = currentUserId,
            LoanAmount = request.LoanAmount,
            SettledAmount = 0,
            Reason = request.Reason,
            ExpectedReturnDate = request.ExpectedReturnDate,
            Status = 0
        };
        await _db.Insertable(loan).ExecuteCommandAsync();
        return loan.Id;
    }

    /// <summary>
    /// <summary>
    /// 审批通过
    /// </summary>
    public async Task ApproveAsync(long id, long currentUserId)
    {
        var loan = await _db.Queryable<ExpenseLoan>().FirstAsync(l => l.Id == id) ?? throw new NotFoundException("借款申请不存在");
        if (loan.Status != 0) throw new BusinessException("非待审批状态");
        loan.Status = 1; // 已借出
        await _db.Updateable(loan).UpdateColumns(l => l.Status).ExecuteCommandAsync();
    }

    /// <summary>
    /// <summary>
    /// 审批驳回
    /// </summary>
    public async Task RejectAsync(long id)
    {
        var loan = await _db.Queryable<ExpenseLoan>().FirstAsync(l => l.Id == id) ?? throw new NotFoundException("借款申请不存在");
        if (loan.Status != 0) throw new BusinessException("非待审批状态");
        loan.Status = 3;
        await _db.Updateable(loan).UpdateColumns(l => l.Status).ExecuteCommandAsync();
    }

    /// <summary>核销借款（从报销单抵扣）</summary>
    public async Task SettleAsync(long loanId, decimal amount, long claimId)
    {
        var loan = await _db.Queryable<ExpenseLoan>().FirstAsync(l => l.Id == loanId) ?? throw new NotFoundException("借款记录不存在");
        if (loan.Status != 1) throw new BusinessException("仅已借出状态可核销");
        if (amount <= 0 || amount > loan.LoanAmount - loan.SettledAmount)
            throw new BusinessException($"核销金额无效，剩余可核销{loan.LoanAmount - loan.SettledAmount}");

        loan.SettledAmount += amount;
        if (loan.SettledAmount >= loan.LoanAmount) loan.Status = 2; // 已核销
        await _db.Updateable(loan).UpdateColumns(l => new { l.SettledAmount, l.Status }).ExecuteCommandAsync();
    }
}
