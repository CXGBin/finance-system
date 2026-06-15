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
    /// <summary>我的待办</summary>
    Task<List<ApprovalInstance>> GetMyPendingAsync(long userId, string? moduleType = null);
    /// <summary>我的已办</summary>
    Task<List<ApprovalInstance>> GetMyDoneAsync(long userId, string? moduleType = null);
    /// <summary>我的申请</summary>
    Task<PageResult<ApprovalInstance>> GetMyInitiatedAsync(long userId, ApprovalInstanceQuery query);
    /// <summary>审批统计</summary>
    Task<object> GetStatisticsAsync(long userId);
    /// <summary>批量审批操作</summary>
    Task BatchActionAsync(List<ApprovalActionRequest> requests, long currentUserId);
    /// <summary>转办（将当前节点转给其他人处理）</summary>
    Task TransferAsync(long instanceId, long targetUserId, string? comment, long currentUserId);
}

/// <summary>
/// 审批流程服务实现
/// </summary>
public class ApprovalFlowService : IApprovalFlowService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// ApprovalFlowService方法</summary>
    public ApprovalFlowService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    /// <summary>
    /// GetListAsync方法</summary>
    public async Task<List<ApprovalFlow>> GetListAsync(string? moduleType = null)
    {
        return await _db.Queryable<ApprovalFlow>()
            .WhereIF(!string.IsNullOrEmpty(moduleType), f => f.ModuleType == moduleType)
            .OrderBy(f => f.CreatedTime, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// CreateAsync方法</summary>
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
    /// <summary>
    /// UpdateAsync方法</summary>
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
    /// <summary>
    /// DeleteAsync方法</summary>
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

    /// <summary>
    /// ApprovalInstanceService方法</summary>
    public ApprovalInstanceService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    /// <summary>
    /// GetListAsync方法</summary>
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
    /// <summary>
    /// StartAsync方法</summary>
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
    /// <summary>
    /// ActionAsync方法</summary>
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
    /// <summary>
    /// WithdrawAsync方法</summary>
    public async Task WithdrawAsync(long instanceId, long currentUserId)
    {
        var instance = await _db.Queryable<ApprovalInstance>().FirstAsync(i => i.Id == instanceId)
            ?? throw new NotFoundException("审批实例不存在");

        if (instance.InitiatorId != currentUserId) throw new BusinessException("仅发起人可撤回");
        if (instance.Status != 0) throw new BusinessException("仅审批中的单据可撤回");

        instance.Status = 3; // 已撤回
        await _db.Updateable(instance).UpdateColumns(i => i.Status).ExecuteCommandAsync();

        // 记录撤回操作
        var currentNode = instance.CurrentNodeIndex;
        var flow = await _db.Queryable<ApprovalFlow>().FirstAsync(f => f.Id == instance.FlowId);
        if (flow != null)
        {
            var nodes = JsonSerializer.Deserialize<List<ApprovalNodeDef>>(flow.NodesJson) ?? new();
            var nodeName = nodes.ElementAtOrDefault(currentNode)?.NodeName ?? "未知节点";
            await _db.Insertable(new ApprovalRecord
            {
                InstanceId = instanceId, NodeIndex = currentNode, NodeName = nodeName,
                ApproverId = currentUserId, Action = 4, Comment = "发起人撤回"
            }).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// GetRecordsAsync方法</summary>
    public async Task<List<ApprovalRecord>> GetRecordsAsync(long instanceId)
    {
        return await _db.Queryable<ApprovalRecord>()
            .Where(r => r.InstanceId == instanceId)
            .OrderBy(r => r.NodeIndex)
            .ToListAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// GetMyPendingAsync方法</summary>
    public async Task<List<ApprovalInstance>> GetMyPendingAsync(long userId, string? moduleType = null)
    {
        // 查询已审批记录的实例ID，排除已处理的
        var approvedInstanceIds = await _db.Queryable<ApprovalRecord>()
            .Where(r => r.ApproverId == userId)
            .Select(r => r.InstanceId)
            .Distinct()
            .ToListAsync();

        var query = _db.Queryable<ApprovalInstance>()
            .Where(i => i.Status == 0 && !approvedInstanceIds.Contains(i.Id));

        if (!string.IsNullOrEmpty(moduleType))
            query = query.Where(i => i.ModuleType == moduleType);

        return await query
            .OrderBy(i => i.CreatedTime, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// GetMyDoneAsync方法</summary>
    public async Task<List<ApprovalInstance>> GetMyDoneAsync(long userId, string? moduleType = null)
    {
        var instanceIds = await _db.Queryable<ApprovalRecord>()
            .Where(r => r.ApproverId == userId)
            .Select(r => r.InstanceId)
            .Distinct()
            .ToListAsync();

        var query = _db.Queryable<ApprovalInstance>()
            .Where(i => instanceIds.Contains(i.Id) && i.Status != 0);

        if (!string.IsNullOrEmpty(moduleType))
            query = query.Where(i => i.ModuleType == moduleType);

        return await query
            .OrderBy(i => i.CreatedTime, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// GetMyInitiatedAsync方法</summary>
    public async Task<PageResult<ApprovalInstance>> GetMyInitiatedAsync(long userId, ApprovalInstanceQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<ApprovalInstance>()
            .Where(i => i.InitiatorId == userId)
            .WhereIF(!string.IsNullOrEmpty(query.ModuleType), i => i.ModuleType == query.ModuleType)
            .WhereIF(query.Status.HasValue, i => i.Status == query.Status)
            .OrderBy(i => i.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<ApprovalInstance>(total, list);
    }

    /// <inheritdoc/>
    /// <summary>
    /// GetStatisticsAsync方法</summary>
    public async Task<object> GetStatisticsAsync(long userId)
    {
        var pendingCount = await _db.Queryable<ApprovalRecord>()
            .Where(r => r.ApproverId == userId && r.Action == 0).CountAsync();

        var approvedInstanceIds = await _db.Queryable<ApprovalRecord>()
            .Where(r => r.ApproverId == userId)
            .Select(r => r.InstanceId)
            .Distinct()
            .ToListAsync();

        var doneCount = await _db.Queryable<ApprovalInstance>()
            .Where(i => approvedInstanceIds.Contains(i.Id) && i.Status != 0)
            .CountAsync();

        var initiatedCount = await _db.Queryable<ApprovalInstance>()
            .Where(i => i.InitiatorId == userId)
            .CountAsync();

        return new { pendingCount, doneCount, initiatedCount };
    }

    /// <inheritdoc/>
    /// <summary>
    /// BatchActionAsync方法</summary>
    public async Task BatchActionAsync(List<ApprovalActionRequest> requests, long currentUserId)
    {
        foreach (var req in requests)
        {
            await ActionAsync(req, currentUserId);
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// TransferAsync方法</summary>
    public async Task TransferAsync(long instanceId, long targetUserId, string? comment, long currentUserId)
    {
        var instance = await _db.Queryable<ApprovalInstance>().FirstAsync(i => i.Id == instanceId)
            ?? throw new NotFoundException("审批实例不存在");
        if (instance.Status != 0) throw new BusinessException("该审批不在进行中");

        // 记录转办操作
        var flow = await _db.Queryable<ApprovalFlow>().FirstAsync(f => f.Id == instance.FlowId)
            ?? throw new NotFoundException("审批流程不存在");
        var nodes = JsonSerializer.Deserialize<List<ApprovalNodeDef>>(flow.NodesJson) ?? new();
        var currentNode = nodes.ElementAtOrDefault(instance.CurrentNodeIndex) ?? throw new BusinessException("当前节点不存在");

        await _db.Insertable(new ApprovalRecord
        {
            InstanceId = instanceId, NodeIndex = instance.CurrentNodeIndex, NodeName = currentNode.NodeName,
            ApproverId = currentUserId, Action = 3, Comment = comment ?? "转办"
        }).ExecuteCommandAsync();

        // 不推进节点，仅标记记录；targetUserId收到待办（通过GetMyPending过滤逻辑自然生效）
        // 转办后目标用户可在其待办列表看到此审批
    }
}
