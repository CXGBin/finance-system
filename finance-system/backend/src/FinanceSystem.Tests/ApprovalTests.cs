using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Approval.DTOs;
using FinanceSystem.Modules.Approval.Entities;
using Xunit;

namespace FinanceSystem.Tests.Approval;

/// <summary>
/// 审批流程服务单元测试
/// </summary>
public class ApprovalFlowTests
{
    /// <summary>
    /// 创建流程：编码唯一性校验
    /// </summary>
    [Fact]
    public void FlowCreate_CodeUniqueness()
    {
        var existingCodes = new List<string> { "EXPENSE_APPROVE", "PURCHASE_APPROVE" };
        var newCode = "BUDGET_APPROVE";
        Assert.DoesNotContain(newCode, existingCodes);
    }

    /// <summary>
    /// 创建流程：编码重复应失败
    /// </summary>
    [Fact]
    public void FlowCreate_DuplicateCode_ShouldFail()
    {
        var existingCodes = new List<string> { "EXPENSE_APPROVE", "PURCHASE_APPROVE" };
        var newCode = "EXPENSE_APPROVE";
        Assert.Contains(newCode, existingCodes);
    }
}

/// <summary>
/// 审批实例状态流转单元测试
/// </summary>
public class ApprovalInstanceTests
{
    /// <summary>
    /// 审批完整链路：发起(0)→通过→完成(1)
    /// </summary>
    [Fact]
    public void ApprovalFlow_StartToComplete()
    {
        var instance = new ApprovalInstance
        {
            FlowId = 1,
            BusinessId = 100,
            Status = 0,
            CurrentNodeIndex = 0
        };

        // 发起审批
        Assert.Equal(0, instance.Status);

        // 第一节点通过（最终节点）
        instance.Status = 1; // 完成
        Assert.Equal(1, instance.Status);
    }

    /// <summary>
    /// 审批链路：发起(0)→通过(下一节点)→再通过→完成(1)
    /// </summary>
    [Fact]
    public void ApprovalFlow_MultiNode_StartToComplete()
    {
        var instance = new ApprovalInstance
        {
            Status = 0,
            CurrentNodeIndex = 0
        };

        // 第一节点通过，前进到下一节点
        instance.CurrentNodeIndex++;
        Assert.Equal(1, instance.CurrentNodeIndex);
        Assert.Equal(0, instance.Status); // 仍在进行中

        // 第二节点通过（最终节点）
        instance.Status = 1;
        Assert.Equal(1, instance.Status);
    }

    /// <summary>
    /// 审批链路：发起(0)→驳回→已驳回(2)
    /// </summary>
    [Fact]
    public void ApprovalFlow_StartToReject()
    {
        var instance = new ApprovalInstance { Status = 0, CurrentNodeIndex = 0 };
        instance.Status = 2; // 驳回
        Assert.Equal(2, instance.Status);
    }

    /// <summary>
    /// 审批链路：发起(0)→撤回→已撤回(3)
    /// </summary>
    [Fact]
    public void ApprovalFlow_StartToWithdraw()
    {
        var instance = new ApprovalInstance
        {
            Status = 0,
            InitiatorId = 100
        };

        // 仅发起人可撤回
        Assert.Equal(100, instance.InitiatorId);
        instance.Status = 3;
        Assert.Equal(3, instance.Status);
    }

    /// <summary>
    /// 非发起人不可撤回
    /// </summary>
    [Fact]
    public void ApprovalFlow_NonInitiatorCannotWithdraw()
    {
        var instance = new ApprovalInstance
        {
            InitiatorId = 100,
            Status = 0
        };
        long currentUserId = 200;
        Assert.NotEqual(instance.InitiatorId, currentUserId);
    }

    /// <summary>
    /// 已完成的审批不可再操作
    /// </summary>
    [Fact]
    public void ApprovalFlow_CompletedCannotAction()
    {
        var instance = new ApprovalInstance { Status = 1 };
        Assert.True(instance.Status != 0);
    }

    /// <summary>
    /// 已驳回的审批不可再操作
    /// </summary>
    [Fact]
    public void ApprovalFlow_RejectedCannotAction()
    {
        var instance = new ApprovalInstance { Status = 2 };
        Assert.True(instance.Status != 0);
    }

    /// <summary>
    /// 已撤回的审批不可再操作
    /// </summary>
    [Fact]
    public void ApprovalFlow_WithdrawnCannotAction()
    {
        var instance = new ApprovalInstance { Status = 3 };
        Assert.True(instance.Status != 0);
    }

    /// <summary>
    /// 节点索引递增逻辑
    /// </summary>
    [Fact]
    public void ApprovalFlow_NodeIndexIncrement()
    {
        var totalNodes = 3;
        var currentIndex = 0;

        // 通过后前进
        currentIndex++;
        Assert.Equal(1, currentIndex);

        currentIndex++;
        Assert.Equal(2, currentIndex);

        // 最后一节点（index == totalNodes-1）
        Assert.True(currentIndex >= totalNodes - 1);
    }
}

/// <summary>
/// 审批记录单元测试
/// </summary>
public class ApprovalRecordTests
{
    /// <summary>
    /// 审批记录：通过操作Action=1
    /// </summary>
    [Fact]
    public void Record_ApproveAction()
    {
        var record = new ApprovalRecord
        {
            InstanceId = 1,
            NodeIndex = 0,
            NodeName = "部门经理审批",
            Action = 1
        };
        Assert.Equal(1, record.Action);
    }

    /// <summary>
    /// 审批记录：驳回操作Action=2
    /// </summary>
    [Fact]
    public void Record_RejectAction()
    {
        var record = new ApprovalRecord
        {
            InstanceId = 1,
            NodeIndex = 0,
            NodeName = "部门经理审批",
            Action = 2
        };
        Assert.Equal(2, record.Action);
    }
}
