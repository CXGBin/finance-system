using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Approval.DTOs;
using FinanceSystem.Modules.Approval.Entities;
using FinanceSystem.Modules.Approval.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Approval.Controllers;

/// <summary>
/// 审批流程管理控制器
/// </summary>
[ApiController]
[Route("api/approval/flow")]
/// <summary>
/// 审批流程定义控制器
/// </summary>
public class ApprovalFlowController : ControllerBase
{
    private readonly IApprovalFlowService _flowService;

    public ApprovalFlowController(IApprovalFlowService flowService) => _flowService = flowService;

    /// <summary>
    /// 获取流程列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<ApprovalFlow>>> GetList([FromQuery] string? moduleType = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string? sortField = null, [FromQuery] string? sortOrder = null)
    {
        return ApiResult<PageResult<ApprovalFlow>>.Success(await _flowService.GetListAsync(moduleType, pageIndex, pageSize, sortField, sortOrder));
    }

    /// <summary>
    /// 创建流程
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] ApprovalFlowRequest request)
    {
        return ApiResult<long>.Success(await _flowService.CreateAsync(request));
    }

    /// <summary>
    /// 修改流程
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] ApprovalFlowRequest request)
    {
        await _flowService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除流程
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _flowService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 审批实例控制器
/// </summary>
[ApiController]
[Route("api/approval/instance")]
/// <summary>
/// 审批实例控制器
/// </summary>
public class ApprovalInstanceController : ControllerBase
{
    private readonly IApprovalInstanceService _instanceService;

    public ApprovalInstanceController(IApprovalInstanceService instanceService) => _instanceService = instanceService;

    /// <summary>
    /// 分页查询审批实例
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<ApprovalInstance>>> GetList([FromQuery] ApprovalInstanceQuery query)
    {
        return ApiResult<PageResult<ApprovalInstance>>.Success(await _instanceService.GetListAsync(query));
    }

    /// <summary>
    /// 查询审批实例详情
    /// </summary>
    [HttpGet("{instanceId}")]
    public async Task<ApiResult<ApprovalInstance>> GetById(long instanceId)
    {
        var instance = await _instanceService.GetByIdAsync(instanceId);
        return instance == null ? ApiResult<ApprovalInstance>.Fail("审批实例不存在") : ApiResult<ApprovalInstance>.Success(instance);
    }

    /// <summary>
    /// 发起审批
    /// </summary>
    [HttpPost("start")]
    public async Task<ApiResult<long>> Start([FromBody] ApprovalStartRequest request)
    {
        return ApiResult<long>.Success(await _instanceService.StartAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 审批操作
    /// </summary>
    [HttpPost("action")]
    public async Task<ApiResult<bool>> Action([FromBody] ApprovalActionRequest request)
    {
        await _instanceService.ActionAsync(request, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 撤回审批
    /// </summary>
    [HttpPost("{instanceId}/withdraw")]
    public async Task<ApiResult<bool>> Withdraw(long instanceId)
    {
        await _instanceService.WithdrawAsync(instanceId, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 获取审批记录
    /// </summary>
    [HttpGet("{instanceId}/records")]
    public async Task<ApiResult<List<ApprovalRecord>>> GetRecords(long instanceId)
    {
        return ApiResult<List<ApprovalRecord>>.Success(await _instanceService.GetRecordsAsync(instanceId));
    }

    /// <summary>
    /// 我的待办（我需要审批的）
    /// </summary>
    [HttpGet("my-pending")]
    public async Task<ApiResult<List<ApprovalInstance>>> MyPending([FromQuery] string? moduleType = null)
    {
        var userId = HttpContext.GetCurrentUserId();
        return ApiResult<List<ApprovalInstance>>.Success(await _instanceService.GetMyPendingAsync(userId, moduleType));
    }

    /// <summary>
    /// 我的已办（我已经审批过的）
    /// </summary>
    [HttpGet("my-done")]
    public async Task<ApiResult<PageResult<ApprovalInstance>>> MyDone([FromQuery] ApprovalInstanceQuery query)
    {
        var userId = HttpContext.GetCurrentUserId();
        return ApiResult<PageResult<ApprovalInstance>>.Success(await _instanceService.GetMyDoneAsync(userId, query));
    }

    /// <summary>
    /// 我的申请（我发起的）
    /// </summary>
    [HttpGet("my-initiated")]
    public async Task<ApiResult<PageResult<ApprovalInstance>>> MyInitiated([FromQuery] ApprovalInstanceQuery query)
    {
        var userId = HttpContext.GetCurrentUserId();
        return ApiResult<PageResult<ApprovalInstance>>.Success(await _instanceService.GetMyInitiatedAsync(userId, query));
    }

    /// <summary>
    /// 审批统计
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ApiResult<object>> Statistics()
    {
        var userId = HttpContext.GetCurrentUserId();
        return ApiResult<object>.Success(await _instanceService.GetStatisticsAsync(userId));
    }

    /// <summary>
    /// 批量审批
    /// </summary>
    [HttpPost("batch-action")]
    public async Task<ApiResult<bool>> BatchAction([FromBody] List<ApprovalActionRequest> requests)
    {
        await _instanceService.BatchActionAsync(requests, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 转办
    /// </summary>
    [HttpPost("{instanceId}/transfer")]
    public async Task<ApiResult<bool>> Transfer(long instanceId, [FromBody] TransferRequest request)
    {
        await _instanceService.TransferAsync(instanceId, request.TargetUserId, request.Comment, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }
}
