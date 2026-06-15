using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Asset.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Asset.DTOs;

/// <summary>资产卡片查询</summary>
public class AssetCardQuery : PageRequest
{
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public long? CategoryId { get; set; }
    public long? DeptId { get; set; }
    public int? Status { get; set; }
}

/// <summary>资产卡片创建/修改</summary>
public class AssetCardRequest
{
    /// <summary>
    /// 资产名称
    /// </summary>
    public string AssetName { get; set; } = string.Empty;
    /// <summary>
    /// 分类ID
    /// </summary>
    public long CategoryId { get; set; }
    public string? Specification { get; set; }
    /// <summary>
    /// 原值
    /// </summary>
    public decimal OriginalValue { get; set; }
    /// <summary>
    /// 残值率（%）
    /// </summary>
    public decimal ResidualRate { get; set; } = 5m;
    /// <summary>
    /// 折旧方法（1直线法 2双倍余额递减法 3年数总和法）
    /// </summary>
    public int DepreciationMethod { get; set; } = 1;
    /// <summary>
    /// 使用月数
    /// </summary>
    public int UsefulLifeMonths { get; set; }
    /// <summary>
    /// 购入日期
    /// </summary>
    public DateTime AcquisitionDate { get; set; }
    public long? DeptId { get; set; }
    public string? Keeper { get; set; }
    public string? Location { get; set; }
    public string? Remark { get; set; }
}

/// <summary>资产分类创建/修改</summary>
public class AssetCategoryRequest
{
    public long? ParentId { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    /// <summary>
    /// 折旧方法（1直线法 2双倍余额递减法 3年数总和法）
    /// </summary>
    public int DepreciationMethod { get; set; } = 1;
    /// <summary>
    /// 使用月数
    /// </summary>
    public int UsefulLifeMonths { get; set; }
    /// <summary>
    /// 残值率（%）
    /// </summary>
    public decimal ResidualRate { get; set; } = 5m;
    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>资产变动请求</summary>
public class AssetChangeRequest
{
    public long AssetCardId { get; set; }
    public int ChangeType { get; set; }
    /// <summary>
    /// 原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    public long? FromDeptId { get; set; }
    public long? ToDeptId { get; set; }
    public decimal? DisposalIncome { get; set; }
}

/// <summary>资产盘点请求</summary>
public class AssetInventoryRequest
{
    /// <summary>
    /// 盘点单号
    /// </summary>
    public string InventoryNo { get; set; } = string.Empty;
    /// <summary>
    /// 盘点日期
    /// </summary>
    public DateTime InventoryDate { get; set; }
    /// <summary>
    /// 操作人ID
    /// </summary>
    public long OperatorId { get; set; }

    /// <summary>
    /// 盘点明细项列表
    /// </summary>
    public List<AssetInventoryItem> Items { get; set; } = new();
}

/// <summary>盘点项</summary>
public class AssetInventoryItem
{
    public long AssetCardId { get; set; }
    public int Result { get; set; } // 1正常 2盘亏 3盘盈
    public decimal? ActualQuantity { get; set; }
    public string? Remark { get; set; }
}

/// <summary>资产报表查询</summary>
public class AssetReportQuery : PageRequest
{
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public long? CategoryId { get; set; }
    public int? Status { get; set; }
}

/// <summary>资产处置请求</summary>
public class AssetDisposeRequest
{
    /// <summary>处置方式：5=报废 4=处置出售</summary>
    public int DisposeType { get; set; }
    /// <summary>处置收入（出售时填写）</summary>
    public decimal DisposalIncome { get; set; }
    /// <summary>处置原因</summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>处置日期</summary>
    public DateTime DisposeDate { get; set; }
}
