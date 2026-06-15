namespace FinanceSystem.Modules.Approval.DTOs;

/// <summary>
/// 审批流程创建/修改
/// </summary>
public class ApprovalFlowRequest
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string FlowName { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码
    /// </summary>
    public string FlowCode { get; set; } = string.Empty;

    /// <summary>
    /// 关联模块
    /// </summary>
    public string ModuleType { get; set; } = string.Empty;

    /// <summary>
    /// 流程描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 节点定义JSON
    /// </summary>
    public string NodesJson { get; set; } = "[]";
}

/// <summary>
/// 审批发起请求
/// </summary>
public class ApprovalStartRequest
{
    /// <summary>
    /// 流程ID
    /// </summary>
    public long FlowId { get; set; }

    /// <summary>
    /// 关联业务ID
    /// </summary>
    public long BusinessId { get; set; }

    /// <summary>
    /// 关联模块
    /// </summary>
    public string ModuleType { get; set; } = string.Empty;

    /// <summary>
    /// 审批标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// 审批操作请求
/// </summary>
public class ApprovalActionRequest
{
    /// <summary>
    /// 审批实例ID
    /// </summary>
    public long InstanceId { get; set; }

    /// <summary>
    /// 审批动作：1通过 2驳回
    /// </summary>
    public int Action { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// 审批实例查询
/// </summary>
public class ApprovalInstanceQuery
{
    /// <summary>
    /// 关联模块
    /// </summary>
    public string? ModuleType { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 审批节点定义
/// </summary>
public class ApprovalNodeDef
{
    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 审批人角色ID（为空表示按岗位）
    /// </summary>
    public long? RoleId { get; set; }

    /// <summary>
    /// 审批人岗位ID（为空表示按角色）
    /// </summary>
    public long? PostId { get; set; }

    /// <summary>
    /// 是否为最终节点（通过后自动结束）
    /// </summary>
    public bool IsFinal { get; set; }
}

/// <summary>转办请求</summary>
public class TransferRequest
{
    public long TargetUserId { get; set; }
    public string? Comment { get; set; }
}
