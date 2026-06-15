using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Budget.Entities;

/// <summary>
/// 预算年度
/// </summary>
[SugarTable("fm_budget_year", "预算年度表")]
public class BudgetYear : FullEntity
{
    /// <summary>
    /// 预算年度
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预算年度")]
    /// <summary>
    /// 预算年度
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// 状态：0草稿 1审批中 2已批准 3执行中 4已冻结
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "状态")]
    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "说明")]
    /// <summary>
    /// 说明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建人ID")]
    /// <summary>
    /// 创建人ID
    /// </summary>
    public long CreatedBy { get; set; }
}

/// <summary>
/// 预算科目（科目维度预算设置）
/// </summary>
[SugarTable("fm_budget_subject", "预算科目表")]
public class BudgetSubject : FullEntity
{
    /// <summary>
    /// 预算年度ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预算年度ID")]
    /// <summary>
    /// 预算年度ID
    /// </summary>
    public long BudgetYearId { get; set; }

    /// <summary>
    /// 关联会计科目ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "会计科目ID")]
    /// <summary>
    /// 会计科目ID
    /// </summary>
    public long SubjectId { get; set; }

    /// <summary>
    /// 关联部门ID（为空表示全公司级别）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "部门ID")]
    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 年度预算总额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "年度预算总额")]
    /// <summary>
    /// 年度预算总额
    /// </summary>
    public decimal AnnualAmount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 月度预算明细
/// </summary>
[SugarTable("fm_budget_monthly", "月度预算明细表")]
public class BudgetMonthly : FullEntity
{
    /// <summary>
    /// 预算科目ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预算科目ID")]
    /// <summary>
    /// 预算科目ID
    /// </summary>
    public long BudgetSubjectId { get; set; }

    /// <summary>
    /// 月份(1-12)
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "月份")]
    /// <summary>
    /// 月份
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// 月度预算金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "月度预算金额")]
    /// <summary>
    /// 月度预算金额
    /// </summary>
    public decimal Amount { get; set; }
}

/// <summary>
/// 预算调整记录
/// </summary>
[SugarTable("fm_budget_adjustment", "预算调整记录表")]
public class BudgetAdjustment : FullEntity
{
    /// <summary>
    /// 预算科目ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预算科目ID")]
    /// <summary>
    /// 预算科目ID
    /// </summary>
    public long BudgetSubjectId { get; set; }

    /// <summary>
    /// 调整类型：1追加 2调减
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "调整类型")]
    /// <summary>
    /// 调整类型
    /// </summary>
    public int AdjustType { get; set; }

    /// <summary>
    /// 调整前金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "调整前金额")]
    /// <summary>
    /// 调整前金额
    /// </summary>
    public decimal BeforeAmount { get; set; }

    /// <summary>
    /// 调整后金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "调整后金额")]
    /// <summary>
    /// 调整后金额
    /// </summary>
    public decimal AfterAmount { get; set; }

    /// <summary>
    /// 调整原因
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false, ColumnDescription = "调整原因")]
    /// <summary>
    /// 调整原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 审批状态：0待审批 1已通过 2已驳回
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "审批状态")]
    /// <summary>
    /// 审批状态
    /// </summary>
    public int ApproveStatus { get; set; }

    /// <summary>
    /// 申请部门ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "申请部门ID")]
    /// <summary>
    /// 申请部门ID
    /// </summary>
    public long? ApplyDeptId { get; set; }

    /// <summary>
    /// 申请人ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "申请人ID")]
    /// <summary>
    /// 申请人ID
    /// </summary>
    public long ApplyBy { get; set; }
}

/// <summary>
/// 预算预警配置
/// </summary>
[SugarTable("fm_budget_alert_config", "预算预警配置表")]
public class BudgetAlertConfig : FullEntity
{
    /// <summary>
    /// 预算年度ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预算年度ID")]
    /// <summary>
    /// 预算年度ID
    /// </summary>
    public long BudgetYearId { get; set; }

    /// <summary>
    /// 预警阈值（百分比，如80表示预算执行到80%时预警）
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "预警阈值百分比")]
    /// <summary>
    /// 预警阈值百分比
    /// </summary>
    public decimal AlertThreshold { get; set; } = 80m;

    /// <summary>
    /// 是否启用预警
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 预警方式：1系统通知 2邮件
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "预警方式")]
    /// <summary>
    /// 预警方式
    /// </summary>
    public int AlertMethod { get; set; } = 1;
}
