using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Approval.DTOs;
using FinanceSystem.Modules.Approval.Entities;
using SqlSugar;
using System.Text.Json;

namespace FinanceSystem.Modules.Approval.Services;

/// <summary>
/// 审批流程服务接口
/// </summary>
public interface IApprovalFlowService
{
    /// <summary>获取流程列表</summary>
    Task<List<ApprovalFlow>> GetListAsync(string? moduleType = null);
    /// <summary>创建流程</summary>
    Task<long> CreateAsync(ApprovalFlowRequest request);
    /// <summary>修改流程</summary>
    Task UpdateAsync(long id, ApprovalFlowRequest request);
    /// <summary>删除流程</summary>
    Task DeleteAsync(long id);
}

/// <summary>
/// 审批实例服务接口
/// </summary>
public interface IApprovalInstanceService
{
    /// <summary>分页查询审批实例</summary>
    Task<PageResult<ApprovalInstance>> GetListAsync(ApprovalInstanceQuery query);
    /// <summary>发起审批</summary>
    Task<long> StartAsync(ApprovalStartRequest request, long currentUserId);
    /// <summary>审批操作（通过/驳回）</summary>
    Task ActionAsync(ApprovalActionRequest request, long currentUserId);
    /// <summary>撤回审批</summary>
    Task WithdrawAsync(long instanceId, long currentUserId);
    /// <summary>获取审批记录</summary>
    Task<List<ApprovalRecord>> GetRecordsAsync(long instanceId);
}

/// <summary>
/// 审批流程服务实现
/// </summary>
public class ApprovalFlowService : IApprovalFlowService
{
    private readonly ISqlSugarClient _db;

    public ApprovalFlowService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<ApprovalFlow>> GetListAsync(string? moduleType = null)
    {
        return await _db.Queryable<ApprovalFlow>()
            .WhereIF(!string.IsNullOrEmpty(moduleType), f => f.ModuleType == moduleType)
            .OrderBy(f => f.CreatedTime, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(ApprovalFlowRequest request)
    {
        var exists = await _db.Queryable<ApprovalFlow>().AnyAsync(f => f.FlowCode == request.FlowCode);
        if (exists) throw new BusinessException($"流程编码 '{request.FlowCode}' 已存在");

        // 验证节点定义至少有一个节点
        var nodes = JsonSerializer.Deserialize<List<ApprovalNodeDef>>(request.NodesJson);
        if (nodes == null || nodes.Count == 0)
            throw new BusinessException("审批流程至少需要一个节点");

        var entity = new ApprovalFlow
        {
            FlowName = request.FlowName,
            FlowCode = request.FlowCode,
            ModuleType = request.ModuleType,
            Description = request.Description,
            IsEnabled = request.IsEnabled,
            NodesJson = request.NodesJson
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, ApprovalFlowRequest request)
    {
        var entity = await _db.Queryable<ApprovalFlow>().FirstAsync(f => f.Id == id)
            ?? throw new NotFoundException("审批流程不存在");
        entity.FlowName = request.FlowName;
        entity.ModuleType = request.ModuleType;
        entity.Description = request.Description;
        entity.IsEnabled = request.IsEnabled;
        entity.NodesJson = request.NodesJson;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        await _db.Deleteable<ApprovalFlow>().Where(f => f.Id == id).ExecuteCommandAsync();
    }
}

/// <summary>
/// 审批实例服务实现
/// </summary>
public class ApprovalInstanceService : IApprovalInstanceService
{
    private readonly ISqlSugarClient _db;

    public ApprovalInstanceService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<ApprovalInstance>> GetListAsync(ApprovalInstanceQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<ApprovalInstance>()
            .WhereIF(!string.IsNullOrEmpty(query.ModuleType), i => i.ModuleType == query.ModuleType)
            .WhereIF(query.Status.HasValue, i => i.Status == query.Status)
            .OrderBy(i => i.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<ApprovalInstance>(total, list);
    }

    /// <inheritdoc/>
    public async Task<long> StartAsync(ApprovalStartRequest request, long currentUserId)
    {
        var flow = await _db.Queryable<ApprovalFlow>().FirstAsync(f => f.Id == request.FlowId)
            ?? throw new NotFoundException("审批流程不存在");

        if (flow.IsEnabled != 1) throw new BusinessException("审批流程未启用");

        // 检查是否已有进行中的审批
        var exists = await _db.Queryable<ApprovalInstance>()
            .AnyAsync(i => i.FlowId == request.FlowId && i.BusinessId == request.BusinessId && i.Status == 0);
        if (exists) throw new BusinessException("该业务已发起审批，请勿重复提交");

        var instance = new ApprovalInstance
        {
            FlowId = request.FlowId,
            BusinessId = request.BusinessId,
            ModuleType = request.ModuleType,
            Title = request.Title,
            InitiatorId = currentUserId,
            CurrentNodeIndex = 0,
            Status = 0
        };
        await _db.Insertable(instance).ExecuteCommandAsync();
        return instance.Id;
    }

    /// <inheritdoc/>
    public async Task ActionAsync(ApprovalActionRequest request, long currentUserId)
    {
        var instance = await _db.Queryable<ApprovalInstance>().FirstAsync(i => i.Id == request.InstanceId)
            ?? throw new NotFoundException("审批实例不存在");

        if (instance.Status != 0) throw new BusinessException("该审批不在进行中");

        var flow = await _db.Queryable<ApprovalFlow>().FirstAsync(f => f.Id == instance.FlowId)
            ?? throw new NotFoundException("审批流程不存在");

        var nodes = JsonSerializer.Deserialize<List<ApprovalNodeDef>>(flow.NodesJson) ?? new();
        var currentNode = nodes.ElementAtOrDefault(instance.CurrentNodeIndex);
        if (currentNode == null) throw new BusinessException("当前审批节点不存在");

        // 记录审批操作
        var record = new ApprovalRecord
        {
            InstanceId = request.InstanceId,
            NodeIndex = instance.CurrentNodeIndex,
            NodeName = currentNode.NodeName,
            ApproverId = currentUserId,
            Action = request.Action,
            Comment = request.Comment
        };
        await _db.Insertable(record).ExecuteCommandAsync();

        if (request.Action == 1) // 通过
        {
            if (currentNode.IsFinal || instance.CurrentNodeIndex >= nodes.Count - 1)
            {
                instance.Status = 1; // 审批完成
            }
            else
            {
                instance.CurrentNodeIndex++;
            }
        }
        else if (request.Action == 2) // 驳回
        {
            instance.Status = 2;
        }

        await _db.Updateable(instance)
            .UpdateColumns(i => new { i.Status, i.CurrentNodeIndex })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task WithdrawAsync(long instanceId, long currentUserId)
    {
        var instance = await _db.Queryable<ApprovalInstance>().FirstAsync(i => i.Id == instanceId)
            ?? throw new NotFoundException("审批实例不存在");

        if (instance.InitiatorId != currentUserId) throw new BusinessException("仅发起人可撤回");
        if (instance.Status != 0) throw new BusinessException("仅审批中的单据可撤回");

        instance.Status = 3; // 已撤回
        await _db.Updateable(instance).UpdateColumns(i => i.Status).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ApprovalRecord>> GetRecordsAsync(long instanceId)
    {
        return await _db.Queryable<ApprovalRecord>()
            .Where(r => r.InstanceId == instanceId)
            .OrderBy(r => r.NodeIndex)
            .ToListAsync();
    }
}
