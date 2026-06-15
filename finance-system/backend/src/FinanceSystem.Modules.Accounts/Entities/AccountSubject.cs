using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.Accounts.Entities;

/// <summary>
/// 会计科目实体
/// </summary>
[SugarTable("fm_account_subject", "会计科目表")]
public class AccountSubject : BaseEntity
{
    /// <summary>
    /// 科目编码（如1001、1002.01）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 科目名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// 父级科目ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 科目层级
    /// </summary>
    public int SubjectLevel { get; set; }

    /// <summary>
    /// 科目类型（1资产 2负债 3权益 4收入 5费用）
    /// </summary>
    public int SubjectType { get; set; }

    /// <summary>
    /// 余额方向（1借方 2贷方）
    /// </summary>
    public int BalanceDirection { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;

    /// <summary>
    /// 是否现金科目（用于日记账）
    /// </summary>
    public int IsCash { get; set; }

    /// <summary>
    /// 是否银行科目
    /// </summary>
    public int IsBank { get; set; }

    /// <summary>
    /// 辅助核算类型（空/department/project/customer）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? AuxiliaryType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 子科目列表（导航属性，不映射数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<AccountSubject>? Children { get; set; }
}
