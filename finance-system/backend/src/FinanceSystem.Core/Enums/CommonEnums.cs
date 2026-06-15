namespace FinanceSystem.Core.Enums;

/// <summary>
/// 凭证状态枚举
/// </summary>
public enum VoucherStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 已审核
    /// </summary>
    Audited = 1,

    /// <summary>
    /// 已作废
    /// </summary>
    Voided = 2
}

/// <summary>
/// 凭证类型枚举
/// </summary>
public enum VoucherType
{
    /// <summary>
    /// 收款凭证
    /// </summary>
    Receipt = 1,

    /// <summary>
    /// 付款凭证
    /// </summary>
    Payment = 2,

    /// <summary>
    /// 转账凭证
    /// </summary>
    Transfer = 3
}

/// <summary>
/// 科目类型枚举
/// </summary>
public enum SubjectType
{
    /// <summary>
    /// 资产类
    /// </summary>
    Asset = 1,

    /// <summary>
    /// 负债类
    /// </summary>
    Liability = 2,

    /// <summary>
    /// 所有者权益类
    /// </summary>
    Equity = 3,

    /// <summary>
    /// 收入类
    /// </summary>
    Revenue = 4,

    /// <summary>
    /// 费用类
    /// </summary>
    Expense = 5
}

/// <summary>
/// 余额方向枚举
/// </summary>
public enum BalanceDirection
{
    /// <summary>
    /// 借方
    /// </summary>
    Debit = 1,

    /// <summary>
    /// 贷方
    /// </summary>
    Credit = 2
}

/// <summary>
/// 菜单类型枚举
/// </summary>
public enum MenuType
{
    /// <summary>
    /// 目录
    /// </summary>
    Directory = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    Menu = 2,

    /// <summary>
    /// 按钮（权限点）
    /// </summary>
    Button = 3
}

/// <summary>
/// 通用状态枚举
/// </summary>
public enum CommonStatus
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}
