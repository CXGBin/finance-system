using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Asset.Entities;

/// <summary>
/// 资产分类
/// </summary>
[SugarTable("fm_asset_category", "资产分类表")]
public class AssetCategory : FullEntity
{
    /// <summary>父级ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "父级ID")]
    /// <summary>
    /// 父级ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>分类编码</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "分类编码")]
    /// <summary>
    /// 分类编码
    /// </summary>
    public string CategoryCode { get; set; } = string.Empty;

    /// <summary>分类名称</summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "分类名称")]
    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>默认折旧方法：1直线法 2双倍余额递减法 3年数总和法</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "折旧方法")]
    /// <summary>
    /// 折旧方法
    /// </summary>
    public int DepreciationMethod { get; set; } = 1;

    /// <summary>默认使用年限（月）</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "使用年限")]
    /// <summary>
    /// 使用年限
    /// </summary>
    public int UsefulLifeMonths { get; set; }

    /// <summary>默认残值率(%)</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "残值率")]
    /// <summary>
    /// 残值率
    /// </summary>
    public decimal ResidualRate { get; set; } = 5m;

    /// <summary>排序号</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "排序号")]
    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>是否启用</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    /// <summary>
    /// 是否启用
    /// </summary>
    public int IsEnabled { get; set; } = 1;
}

/// <summary>
/// 资产卡片（固定资产）
/// </summary>
[SugarTable("fm_asset_card", "资产卡片表")]
public class AssetCard : FullEntity
{
    /// <summary>资产编号（自动生成）</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "资产编号")]
    /// <summary>
    /// 资产编号
    /// </summary>
    public string AssetCode { get; set; } = string.Empty;

    /// <summary>资产名称</summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "资产名称")]
    /// <summary>
    /// 资产名称
    /// </summary>
    public string AssetName { get; set; } = string.Empty;

    /// <summary>资产分类ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "资产分类ID")]
    /// <summary>
    /// 资产分类ID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>规格型号</summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "规格型号")]
    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>资产原值</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "资产原值")]
    /// <summary>
    /// 资产原值
    /// </summary>
    public decimal OriginalValue { get; set; }

    /// <summary>残值率(%)</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "残值率")]
    /// <summary>
    /// 残值率
    /// </summary>
    public decimal ResidualRate { get; set; } = 5m;

    /// <summary>残值（自动计算）</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "残值")]
    /// <summary>
    /// 残值
    /// </summary>
    public decimal ResidualValue { get; set; }

    /// <summary>折旧方法</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "折旧方法")]
    /// <summary>
    /// 折旧方法
    /// </summary>
    public int DepreciationMethod { get; set; } = 1;

    /// <summary>使用年限（月）</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "使用年限")]
    /// <summary>
    /// 使用年限
    /// </summary>
    public int UsefulLifeMonths { get; set; }

    /// <summary>入账日期</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "入账日期")]
    /// <summary>
    /// 入账日期
    /// </summary>
    public DateTime AcquisitionDate { get; set; }

    /// <summary>使用部门ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "使用部门ID")]
    /// <summary>
    /// 使用部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>保管人</summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "保管人")]
    /// <summary>
    /// 保管人
    /// </summary>
    public string? Keeper { get; set; }

    /// <summary>存放地点</summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "存放地点")]
    /// <summary>
    /// 存放地点
    /// </summary>
    public string? Location { get; set; }

    /// <summary>资产状态：1在用 2闲置 3维修中 4已处置 5已报废</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "资产状态")]
    /// <summary>
    /// 资产状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>累计折旧</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "累计折旧")]
    /// <summary>
    /// 累计折旧
    /// </summary>
    public decimal AccumulatedDepreciation { get; set; }

    /// <summary>净值</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "净值")]
    /// <summary>
    /// 净值
    /// </summary>
    public decimal NetValue { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 资产折旧明细
/// </summary>
[SugarTable("fm_asset_depreciation", "资产折旧明细表")]
public class AssetDepreciation : FullEntity
{
    /// <summary>资产卡片ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "资产卡片ID")]
    /// <summary>
    /// 资产卡片ID
    /// </summary>
    public long AssetCardId { get; set; }

    /// <summary>折旧期间ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "会计期间ID")]
    /// <summary>
    /// 会计期间ID
    /// </summary>
    public long PeriodId { get; set; }

    /// <summary>折旧月份</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "月份")]
    /// <summary>
    /// 月份
    /// </summary>
    public int Month { get; set; }

    /// <summary>本期折旧额</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "本期折旧额")]
    /// <summary>
    /// 本期折旧额
    /// </summary>
    public decimal DepreciationAmount { get; set; }

    /// <summary>折旧后累计折旧</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "累计折旧")]
    /// <summary>
    /// 累计折旧
    /// </summary>
    public decimal AccumulatedDepreciation { get; set; }

    /// <summary>折旧后净值</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = false, ColumnDescription = "净值")]
    /// <summary>
    /// 净值
    /// </summary>
    public decimal NetValue { get; set; }
}

/// <summary>
/// 资产变动记录
/// </summary>
[SugarTable("fm_asset_change", "资产变动记录表")]
public class AssetChange : FullEntity
{
    /// <summary>资产卡片ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "资产卡片ID")]
    /// <summary>
    /// 资产卡片ID
    /// </summary>
    public long AssetCardId { get; set; }

    /// <summary>变动类型：1调拨 2处置 3报废</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "变动类型")]
    /// <summary>
    /// 变动类型
    /// </summary>
    public int ChangeType { get; set; }

    /// <summary>变动原因</summary>
    [SugarColumn(Length = 500, IsNullable = false, ColumnDescription = "变动原因")]
    /// <summary>
    /// 变动原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>变动前部门ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "变动前部门ID")]
    /// <summary>
    /// 变动前部门ID
    /// </summary>
    public long? FromDeptId { get; set; }

    /// <summary>变动后部门ID</summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "变动后部门ID")]
    /// <summary>
    /// 变动后部门ID
    /// </summary>
    public long? ToDeptId { get; set; }

    /// <summary>处置收入</summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "处置收入")]
    /// <summary>
    /// 处置收入
    /// </summary>
    public decimal? DisposalIncome { get; set; }

    /// <summary>操作人ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "操作人ID")]
    /// <summary>
    /// 操作人ID
    /// </summary>
    public long OperatorId { get; set; }
}

/// <summary>
/// 资产盘点单
/// </summary>
[SugarTable("fm_asset_inventory", "资产盘点单表")]
public class AssetInventory : FullEntity
{
    /// <summary>盘点编号</summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "盘点编号")]
    /// <summary>
    /// 盘点编号
    /// </summary>
    public string InventoryNo { get; set; } = string.Empty;

    /// <summary>盘点日期</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "盘点日期")]
    /// <summary>
    /// 盘点日期
    /// </summary>
    public DateTime InventoryDate { get; set; }

    /// <summary>盘点人ID</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "盘点人ID")]
    /// <summary>
    /// 盘点人ID
    /// </summary>
    public long OperatorId { get; set; }

    /// <summary>盘点结果JSON</summary>
    [SugarColumn(Length = 4000, IsNullable = true, ColumnDescription = "盘点明细JSON")]
    /// <summary>
    /// 盘点明细JSON
    /// </summary>
    public string? ItemsJson { get; set; }

    /// <summary>状态：0未完成 1已完成</summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "状态")]
    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 0;
}
