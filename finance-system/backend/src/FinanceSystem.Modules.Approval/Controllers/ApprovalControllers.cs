using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Approval.Services;
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
public class ApprovalFlowController : ControllerBase
{
    private readonly IApprovalFlowService _flowService;

    public ApprovalFlowController(IApprovalFlowService flowService) => _flowService = flowService;

    /// <summary>
    /// 获取流程列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<ApprovalFlow>>> GetList([FromQuery] string? moduleType = null)
    {
        return ApiResult<List<ApprovalFlow>>.Success(await _flowService.GetListAsync(moduleType));
    }

    /// <summary>
    /// 创建流程
    /// </summary>
    [HttpPost]
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
    /// 发起审批
    /// </summary>
    [HttpPost("start")]
    public async Task<ApiResult<long>> Start([FromBody] ApprovalStartRequest request)
    {
        return ApiResult<long>.Success(await _instanceService.StartAsync(request, 0));
    }

    /// <summary>
    /// 审批操作
    /// </summary>
    [HttpPost("action")]
    public async Task<ApiResult<bool>> Action([FromBody] ApprovalActionRequest request)
    {
        await _instanceService.ActionAsync(request, 0);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 撤回审批
    /// </summary>
    [HttpPost("{instanceId}/withdraw")]
    public async Task<ApiResult<bool>> Withdraw(long instanceId)
    {
        await _instanceService.WithdrawAsync(instanceId, 0);
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
}
