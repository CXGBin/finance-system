namespace FinanceSystem.Core.Enums;

/// <summary>
/// 审批状态枚举
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// 审批中
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 已驳回
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// 已撤回
    /// </summary>
    Withdrawn = 3
}

/// <summary>
/// 审批任务状态枚举
/// </summary>
public enum ApprovalTaskStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 已驳回
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// 已转办
    /// </summary>
    Transferred = 3
}

/// <summary>
/// 预算年度状态枚举
/// </summary>
public enum BudgetYearStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 已审批
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 执行中
    /// </summary>
    Executing = 2,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed = 3
}

/// <summary>
/// 报销单状态枚举
/// </summary>
public enum ExpenseClaimStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 待审批
    /// </summary>
    PendingApproval = 1,

    /// <summary>
    /// 审批中
    /// </summary>
    Approving = 2,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 3,

    /// <summary>
    /// 已驳回
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// 已付款
    /// </summary>
    Paid = 5,

    /// <summary>
    /// 已作废
    /// </summary>
    Voided = 6
}

/// <summary>
/// 资产状态枚举
/// </summary>
public enum AssetStatus
{
    /// <summary>
    /// 在用
    /// </summary>
    InUse = 0,

    /// <summary>
    /// 闲置
    /// </summary>
    Idle = 1,

    /// <summary>
    /// 已处置
    /// </summary>
    Disposed = 2,

    /// <summary>
    /// 已报废
    /// </summary>
    Scrapped = 3
}

/// <summary>
/// 纳税申报状态枚举
/// </summary>
public enum TaxDeclarationStatus
{
    /// <summary>
    /// 待申报
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已申报
    /// </summary>
    Declared = 1,

    /// <summary>
    /// 已缴款
    /// </summary>
    Paid = 2
}
