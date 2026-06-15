namespace FinanceSystem.Modules.Budget.DTOs;

/// <summary>
/// 预算年度列表查询
/// </summary>
public class BudgetYearQuery
{
    /// <summary>
    /// 年份筛选
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 预算年度创建/修改
/// </summary>
public class BudgetYearRequest
{
    /// <summary>
    /// 预算年度
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 预算科目创建/修改
/// </summary>
public class BudgetSubjectRequest
{
    /// <summary>
    /// 预算年度ID
    /// </summary>
    public long BudgetYearId { get; set; }

    /// <summary>
    /// 会计科目ID
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 部门ID（可选）
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 年度预算总额
    /// </summary>
    public decimal AnnualAmount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 月度预算保存请求
/// </summary>
public class BudgetMonthlySaveRequest
{
    /// <summary>
    /// 预算科目ID
    /// </summary>
    public long BudgetSubjectId { get; set; }

    /// <summary>
    /// 月度预算列表
    /// </summary>
    public List<BudgetMonthlyItem> Items { get; set; } = new();
}

/// <summary>
/// 月度预算项
/// </summary>
public class BudgetMonthlyItem
{
    /// <summary>
    /// 月份
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }
}

/// <summary>
/// 预算执行跟踪查询
/// </summary>
public class BudgetExecutionQuery
{
    /// <summary>
    /// 预算年度ID
    /// </summary>
    public long BudgetYearId { get; set; }

    /// <summary>
    /// 月份（可选）
    /// </summary>
    public int? Month { get; set; }

    /// <summary>
    /// 部门ID（可选）
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 科目ID（可选）
    /// </summary>
    public long? SubjectId { get; set; }
}

/// <summary>
/// 预算执行跟踪结果行
/// </summary>
public class BudgetExecutionItem
{
    /// <summary>
    /// 预算科目ID
    /// </summary>
    public long BudgetSubjectId { get; set; }

    /// <summary>
    /// 科目编码
    /// </summary>
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 科目名称
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = "全公司";

    /// <summary>
    /// 年度预算
    /// </summary>
    public decimal AnnualBudget { get; set; }

    /// <summary>
    /// 已执行金额
    /// </summary>
    public decimal ExecutedAmount { get; set; }

    /// <summary>
    /// 执行率（百分比）
    /// </summary>
    public decimal ExecutionRate { get; set; }

    /// <summary>
    /// 剩余预算
    /// </summary>
    public decimal RemainingBudget { get; set; }
}

/// <summary>
/// 预算调整请求
/// </summary>
public class BudgetAdjustRequest
{
    /// <summary>
    /// 预算科目ID
    /// </summary>
    public long BudgetSubjectId { get; set; }

    /// <summary>
    /// 调整类型：1追加 2调减
    /// </summary>
    public int AdjustType { get; set; }

    /// <summary>
    /// 调整金额
    /// </summary>
    public decimal AdjustAmount { get; set; }

    /// <summary>
    /// 调整原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 预算预警配置请求
/// </summary>
public class BudgetAlertConfigRequest
{
    /// <summary>
    /// 预算年度ID
    /// </summary>
    public long BudgetYearId { get; set; }

    /// <summary>
    /// 预警阈值百分比
    /// </summary>
    public decimal AlertThreshold { get; set; } = 80m;

    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 预警方式
    /// </summary>
    public int AlertMethod { get; set; } = 1;
}
