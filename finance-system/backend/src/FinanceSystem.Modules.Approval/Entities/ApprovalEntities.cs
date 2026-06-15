using FinanceSystem.Core.Common;
using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Approval.Entities;

/// <summary>
/// 审批流程定义
/// </summary>
[SugarTable("fm_approval_flow", "审批流程定义表")]
public class ApprovalFlow : FullEntity
{
    /// <summary>
    /// 流程名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "流程名称")]
    public string FlowName { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "流程编码")]
    public string FlowCode { get; set; } = string.Empty;

    /// <summary>
    /// 关联模块：budget/expense/asset
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "关联模块")]
    public string ModuleType { get; set; } = string.Empty;

    /// <summary>
    /// 流程描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "流程描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 流程节点定义JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "nvarchar(max)", IsNullable = false, ColumnDescription = "节点定义JSON")]
    public string NodesJson { get; set; } = "[]";
}

/// <summary>
/// 审批实例（发起的审批单）
/// </summary>
[SugarTable("fm_approval_instance", "审批实例表")]
public class ApprovalInstance : FullEntity
{
    /// <summary>
    /// 流程ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "流程ID")]
    public long FlowId { get; set; }

    /// <summary>
    /// 关联业务ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "业务ID")]
    public long BusinessId { get; set; }

    /// <summary>
    /// 关联模块
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "关联模块")]
    public string ModuleType { get; set; } = string.Empty;

    /// <summary>
    /// 审批标题
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "审批标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 发起人ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "发起人ID")]
    public long InitiatorId { get; set; }

    /// <summary>
    /// 当前节点序号
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "当前节点序号")]
    public int CurrentNodeIndex { get; set; }

    /// <summary>
    /// 审批状态：0审批中 1已通过 2已驳回 3已撤回
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批状态")]
    public int Status { get; set; }

    /// <summary>
    /// 申请人部门ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "申请部门ID")]
    public long? DeptId { get; set; }
}

/// <summary>
/// 审批记录（每个节点的审批操作）
/// </summary>
[SugarTable("fm_approval_record", "审批记录表")]
public class ApprovalRecord : FullEntity
{
    /// <summary>
    /// 审批实例ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批实例ID")]
    public long InstanceId { get; set; }

    /// <summary>
    /// 节点序号
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "节点序号")]
    public int NodeIndex { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "节点名称")]
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 审批人ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批人ID")]
    public long ApproverId { get; set; }

    /// <summary>
    /// 审批动作：1通过 2驳回 3转审
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批动作")]
    public int Action { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "审批意见")]
    public string? Comment { get; set; }

    /// <summary>
    /// 审批时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批时间")]
    public DateTime ApproveTime { get; set; } = DateTime.Now;
}
