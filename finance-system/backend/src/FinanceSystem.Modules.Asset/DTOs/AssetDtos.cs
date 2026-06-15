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
    public string AssetName { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public string? Specification { get; set; }
    public decimal OriginalValue { get; set; }
    public decimal ResidualRate { get; set; } = 5m;
    public int DepreciationMethod { get; set; } = 1;
    public int UsefulLifeMonths { get; set; }
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
    public int DepreciationMethod { get; set; } = 1;
    public int UsefulLifeMonths { get; set; }
    public decimal ResidualRate { get; set; } = 5m;
    public int SortOrder { get; set; }
}

/// <summary>资产变动请求</summary>
public class AssetChangeRequest
{
    public long AssetCardId { get; set; }
    public int ChangeType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long? FromDeptId { get; set; }
    public long? ToDeptId { get; set; }
    public decimal? DisposalIncome { get; set; }
}
