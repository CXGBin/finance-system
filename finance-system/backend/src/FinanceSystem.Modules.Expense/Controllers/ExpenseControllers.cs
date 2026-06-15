using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Expense.DTOs;
using FinanceSystem.Modules.Expense.Entities;
using FinanceSystem.Modules.Expense.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Expense.Controllers;

[ApiController]
[Route("api/expense/type")]
/// <summary>
/// 费用类型控制器
/// </summary>
public class ExpenseTypeController : ControllerBase
{
    private readonly IExpenseTypeService _service;
    public ExpenseTypeController(IExpenseTypeService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<List<ExpenseType>>> GetList() => ApiResult<List<ExpenseType>>.Success(await _service.GetListAsync());

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] ExpenseTypeRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] ExpenseTypeRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/expense/claim")]
/// <summary>
/// 费用报销控制器
/// </summary>
public class ExpenseClaimController : ControllerBase
{
    private readonly IExpenseClaimService _service;
    public ExpenseClaimController(IExpenseClaimService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<PageResult<ExpenseClaim>>> GetList([FromQuery] ExpenseClaimQuery query) => ApiResult<PageResult<ExpenseClaim>>.Success(await _service.GetListAsync(query));

    [HttpGet("{id}")]
    public async Task<ApiResult<ExpenseClaim>> GetById(long id) { var r = await _service.GetByIdAsync(id); return r == null ? ApiResult<ExpenseClaim>.Fail("不存在") : ApiResult<ExpenseClaim>.Success(r); }

    /// <summary>
    /// 修改报销单（仅草稿状态）
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] ExpenseClaimRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] ExpenseClaimRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request, HttpContext.GetCurrentUserId()));

    [HttpPost("{id}/submit")]
    public async Task<ApiResult<bool>> Submit(long id) { await _service.SubmitAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/approve")]
    public async Task<ApiResult<bool>> Approve(long id) { await _service.ApproveAsync(id); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/reject")]
    public async Task<ApiResult<bool>> Reject(long id) { await _service.RejectAsync(id); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/pay")]
    public async Task<ApiResult<bool>> Pay(long id) { await _service.ConfirmPaymentAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/expense/statistics")]
/// <summary>
/// 费用统计控制器
/// </summary>
public class ExpenseStatisticsController : ControllerBase
{
    private readonly IExpenseStatisticsService _service;
    public ExpenseStatisticsController(IExpenseStatisticsService service) => _service = service;

    [HttpGet]
    public async Task<ApiResult<List<object>>> GetStatistics([FromQuery] ExpenseStatisticsQuery query) => ApiResult<List<object>>.Success(await _service.GetStatisticsAsync(query));
}

/// <summary>费用分摊控制器</summary>
[ApiController]
[Route("api/expense/allocate")]
/// <summary>
/// 费用分摊控制器
/// </summary>
public class ExpenseAllocateController : ControllerBase
{
    private readonly IExpenseAllocateService _service;
    public ExpenseAllocateController(IExpenseAllocateService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<PageResult<ExpenseAllocate>>> GetList([FromQuery] PageRequest query) => ApiResult<PageResult<ExpenseAllocate>>.Success(await _service.GetListAsync(query));

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] ExpenseAllocateRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));
}

/// <summary>借款申请控制器</summary>
[ApiController]
[Route("api/expense/loan")]
/// <summary>
/// 借款管理控制器
/// </summary>
public class ExpenseLoanController : ControllerBase
{
    private readonly IExpenseLoanService _service;
    public ExpenseLoanController(IExpenseLoanService service) => _service = service;

    [HttpGet]
    public async Task<ApiResult<PageResult<ExpenseLoan>>> GetList([FromQuery] ExpenseLoanQuery query) => ApiResult<PageResult<ExpenseLoan>>.Success(await _service.GetListAsync(query));

    [HttpGet("{id}")]
    public async Task<ApiResult<ExpenseLoan?>> GetById(long id) => ApiResult<ExpenseLoan?>.Success(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] ExpenseLoanRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request, HttpContext.GetCurrentUserId()));

    [HttpPost("{id}/approve")]
    public async Task<ApiResult<bool>> Approve(long id) { await _service.ApproveAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/reject")]
    public async Task<ApiResult<bool>> Reject(long id) { await _service.RejectAsync(id); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/settle")]
    public async Task<ApiResult<bool>> Settle(long id, [FromBody] SettleRequest request) { await _service.SettleAsync(id, request.Amount, request.ClaimId); return ApiResult<bool>.Success(true); }
}

/// <summary>核销请求</summary>
public class SettleRequest
{
    public decimal Amount { get; set; }
    public long ClaimId { get; set; }
}
