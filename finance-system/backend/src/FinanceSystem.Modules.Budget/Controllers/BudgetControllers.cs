using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Budget.Services;
using FinanceSystem.Modules.Budget.DTOs;
using FinanceSystem.Modules.Budget.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Budget.Controllers;

/// <summary>
/// 预算年度管理控制器
/// </summary>
[ApiController]
[Route("api/budget/setting")]
/// <summary>
/// 预算年度控制器
/// </summary>
public class BudgetYearController : ControllerBase
{
    private readonly IBudgetYearService _yearService;

    public BudgetYearController(IBudgetYearService yearService) => _yearService = yearService;

    /// <summary>
    /// 获取预算年度列表
    /// </summary>
    [HttpGet("years")]
    public async Task<ApiResult<List<BudgetYear>>> GetYears([FromQuery] BudgetYearQuery query)
    {
        return ApiResult<List<BudgetYear>>.Success(await _yearService.GetListAsync(query));
    }

    /// <summary>
    /// 创建预算年度
    /// </summary>
    [HttpPost("year")]
    public async Task<ApiResult<long>> CreateYear([FromBody] BudgetYearRequest request)
    {
        return ApiResult<long>.Success(await _yearService.CreateAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 更新年度状态
    /// </summary>
    [HttpPut("year/{id}/status")]
    public async Task<ApiResult<bool>> UpdateStatus(long id, [FromQuery] int status)
    {
        await _yearService.UpdateStatusAsync(id, status);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 预算科目管理控制器
/// </summary>
[ApiController]
[Route("api/budget/setting/subject")]
/// <summary>
/// 预算科目控制器
/// </summary>
public class BudgetSubjectController : ControllerBase
{
    private readonly IBudgetSubjectService _subjectService;

    public BudgetSubjectController(IBudgetSubjectService subjectService) => _subjectService = subjectService;

    /// <summary>
    /// 获取预算科目列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<BudgetSubject>>> GetList([FromQuery] long yearId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string? sortField = null, [FromQuery] string? sortOrder = null)
    {
        return ApiResult<PageResult<BudgetSubject>>.Success(await _subjectService.GetListAsync(yearId, pageIndex, pageSize, sortField, sortOrder));
    }

    /// <summary>
    /// 新增预算科目
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] BudgetSubjectRequest request)
    {
        return ApiResult<long>.Success(await _subjectService.CreateAsync(request));
    }

    /// <summary>
    /// 修改预算科目
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] BudgetSubjectRequest request)
    {
        await _subjectService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除预算科目
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _subjectService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 月度预算控制器
/// </summary>
[ApiController]
[Route("api/budget/plan")]
/// <summary>
/// 月度预算控制器
/// </summary>
public class BudgetMonthlyController : ControllerBase
{
    private readonly IBudgetMonthlyService _monthlyService;

    public BudgetMonthlyController(IBudgetMonthlyService monthlyService) => _monthlyService = monthlyService;

    /// <summary>
    /// 获取月度预算
    /// </summary>
    [HttpGet("{budgetSubjectId}")]
    public async Task<ApiResult<List<BudgetMonthly>>> GetMonthly(long budgetSubjectId)
    {
        return ApiResult<List<BudgetMonthly>>.Success(await _monthlyService.GetBySubjectAsync(budgetSubjectId));
    }

    /// <summary>
    /// 保存月度预算
    /// </summary>
    [HttpPost("monthly")]
    public async Task<ApiResult<bool>> SaveMonthly([FromBody] BudgetMonthlySaveRequest request)
    {
        await _monthlyService.SaveAsync(request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 自动平均拆分年度预算到月度
    /// </summary>
    [HttpPost("auto-split")]
    public async Task<ApiResult<bool>> AutoSplit([FromQuery] long budgetSubjectId)
    {
        await _monthlyService.AutoSplitAsync(budgetSubjectId);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 预算执行跟踪控制器
/// </summary>
[ApiController]
[Route("api/budget/execution")]
/// <summary>
/// 预算执行控制器
/// </summary>
public class BudgetExecutionController : ControllerBase
{
    private readonly IBudgetExecutionService _executionService;

    public BudgetExecutionController(IBudgetExecutionService executionService) => _executionService = executionService;

    /// <summary>
    /// 查询预算执行情况
    /// </summary>
    [HttpGet("")]
    public async Task<ApiResult<List<BudgetExecutionItem>>> GetExecution([FromQuery] BudgetExecutionQuery query)
    {
        return ApiResult<List<BudgetExecutionItem>>.Success(await _executionService.GetExecutionAsync(query));
    }
}

/// <summary>
/// 预算调整控制器
/// </summary>
[ApiController]
[Route("api/budget/adjustment")]
/// <summary>
/// 预算调整控制器
/// </summary>
public class BudgetAdjustmentController : ControllerBase
{
    private readonly IBudgetAdjustService _adjustService;

    public BudgetAdjustmentController(IBudgetAdjustService adjustService) => _adjustService = adjustService;

    /// <summary>
    /// 发起预算调整
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> CreateAdjust([FromBody] BudgetAdjustRequest request)
    {
        return ApiResult<long>.Success(await _adjustService.CreateAdjustAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 审批预算调整
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ApiResult<bool>> ApproveAdjust(long id, [FromQuery] int action)
    {
        await _adjustService.ApproveAdjustAsync(id, action, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 预算预警控制器
/// </summary>
[ApiController]
[Route("api/budget/alert")]
/// <summary>
/// 预算预警控制器
/// </summary>
public class BudgetAlertController : ControllerBase
{
    private readonly IBudgetAlertService _alertService;

    public BudgetAlertController(IBudgetAlertService alertService) => _alertService = alertService;

    /// <summary>
    /// 获取预警配置
    /// </summary>
    [HttpGet("config")]
    public async Task<ApiResult<BudgetAlertConfig?>> GetConfig([FromQuery] long budgetYearId)
    {
        return ApiResult<BudgetAlertConfig?>.Success(await _alertService.GetConfigAsync(budgetYearId));
    }

    /// <summary>
    /// 保存预警配置
    /// </summary>
    [HttpPost("config")]
    public async Task<ApiResult<bool>> SaveConfig([FromBody] BudgetAlertConfigRequest request)
    {
        await _alertService.SaveConfigAsync(request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 检查超预警科目
    /// </summary>
    [HttpGet("check")]
    public async Task<ApiResult<List<BudgetExecutionItem>>> CheckAlerts([FromQuery] long budgetYearId)
    {
        return ApiResult<List<BudgetExecutionItem>>.Success(await _alertService.CheckAlertsAsync(budgetYearId));
    }
}

/// <summary>
/// 预算分析控制器
/// </summary>
[ApiController]
[Route("api/budget/analysis")]
/// <summary>
/// 预算分析控制器
/// </summary>
public class BudgetAnalysisController : ControllerBase
{
    private readonly IBudgetAnalysisService _analysisService;
    public BudgetAnalysisController(IBudgetAnalysisService analysisService) => _analysisService = analysisService;

    /// <summary>科目对比分析（预算vs实际）</summary>
    [HttpGet("subject-compare")]
    public async Task<ApiResult<List<object>>> SubjectCompare([FromQuery] int year)
        => ApiResult<List<object>>.Success(await _analysisService.GetSubjectCompareAsync(year));

    /// <summary>月度执行趋势</summary>
    [HttpGet("monthly-trend")]
    public async Task<ApiResult<List<object>>> MonthlyTrend([FromQuery] int year)
        => ApiResult<List<object>>.Success(await _analysisService.GetMonthlyTrendAsync(year));

    /// <summary>费用TOP10排名</summary>
    [HttpGet("expense-top10")]
    public async Task<ApiResult<List<object>>> ExpenseTop10([FromQuery] int year)
        => ApiResult<List<object>>.Success(await _analysisService.GetExpenseTop10Async(year));

    /// <summary>综合分析概览</summary>
    [HttpGet("overview")]
    public async Task<ApiResult<object>> Overview([FromQuery] int year)
        => ApiResult<object>.Success(await _analysisService.GetOverviewAsync(year));
}
