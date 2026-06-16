using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Asset.DTOs;
using FinanceSystem.Modules.Asset.Entities;
using FinanceSystem.Modules.Asset.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Asset.Controllers;

[ApiController]
[Route("api/asset/category")]
/// <summary>
/// 资产分类控制器
/// </summary>
public class AssetCategoryController : ControllerBase
{
    private readonly IAssetCategoryService _service;
    public AssetCategoryController(IAssetCategoryService service) => _service = service;

    /// <summary>
    /// 获取资产分类树
    /// </summary>
    [HttpGet("tree")]
    public async Task<ApiResult<List<AssetCategory>>> GetTree() => ApiResult<List<AssetCategory>>.Success(await _service.GetTreeAsync());

    /// <summary>
    /// 新增资产分类
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] AssetCategoryRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    /// <summary>
    /// 修改资产分类
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] AssetCategoryRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 删除资产分类
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/asset/card")]
/// <summary>
/// 资产卡片控制器
/// </summary>
public class AssetCardController : ControllerBase
{
    private readonly IAssetCardService _service;
    public AssetCardController(IAssetCardService service) => _service = service;

    /// <summary>
    /// 分页查询资产卡片列表
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<PageResult<AssetCard>>> GetPage([FromQuery] AssetCardQuery query) => ApiResult<PageResult<AssetCard>>.Success(await _service.GetPageAsync(query));

    /// <summary>
    /// 获取资产卡片详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<AssetCard>> GetById(long id) { var r = await _service.GetByIdAsync(id); return r == null ? ApiResult<AssetCard>.Fail("不存在") : ApiResult<AssetCard>.Success(r); }

    /// <summary>
    /// 新增资产卡片
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] AssetCardRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    /// <summary>
    /// 修改资产卡片
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] AssetCardRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 删除资产卡片
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 资产变更（启用/停用/调拨/报废）
    /// </summary>
    [HttpPost("{id}/change")]
    public async Task<ApiResult<bool>> ChangeStatus(long id, [FromQuery] int changeType, [FromBody] AssetChangeRequest request) { await _service.ChangeStatusAsync(id, changeType, request, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/asset/depreciation")]
/// <summary>
/// 资产折旧控制器
/// </summary>
public class AssetDepreciationController : ControllerBase
{
    private readonly IAssetDepreciationService _service;
    public AssetDepreciationController(IAssetDepreciationService service) => _service = service;

    /// <summary>
    /// 计算指定月份折旧
    /// </summary>
    [HttpGet("calculate")]
    public async Task<ApiResult<List<AssetDepreciation>>> Calculate([FromQuery] int year, [FromQuery] int month) => ApiResult<List<AssetDepreciation>>.Success(await _service.CalculateAsync(year, month));

    /// <summary>
    /// 确认折旧入账
    /// </summary>
    [HttpPost("confirm")]
    public async Task<ApiResult<bool>> Confirm([FromQuery] int year, [FromQuery] int month) { await _service.ConfirmDepreciationAsync(year, month, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 获取年度折旧汇总
    /// </summary>
    [HttpGet("summary")]
    public async Task<ApiResult<List<object>>> GetSummary([FromQuery] int year) => ApiResult<List<object>>.Success(await _service.GetSummaryAsync(year));
}

/// <summary>资产盘点控制器</summary>
[ApiController]
[Route("api/asset/inventory")]
/// <summary>
/// 资产盘点控制器
/// </summary>
public class AssetInventoryController : ControllerBase
{
    private readonly IAssetInventoryService _service;
    public AssetInventoryController(IAssetInventoryService service) => _service = service;

    /// <summary>
    /// 分页查询资产盘点列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<AssetInventory>>> GetList([FromQuery] PageRequest query) => ApiResult<PageResult<AssetInventory>>.Success(await _service.GetListAsync(query));

    /// <summary>
    /// 创建资产盘点单
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] AssetInventoryRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    /// <summary>
    /// 完成资产盘点
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ApiResult<bool>> Complete(long id) { await _service.CompleteAsync(id); return ApiResult<bool>.Success(true); }
}

/// <summary>资产报表控制器</summary>
[ApiController]
[Route("api/asset/report")]
/// <summary>
/// 资产报表控制器
/// </summary>
public class AssetReportController : ControllerBase
{
    private readonly IAssetReportService _service;
    public AssetReportController(IAssetReportService service) => _service = service;

    /// <summary>
    /// 获取资产台账
    /// </summary>
    [HttpGet("ledger")]
    public async Task<ApiResult<PageResult<object>>> Ledger([FromQuery] AssetReportQuery query) => ApiResult<PageResult<object>>.Success(await _service.GetLedgerAsync(query));

    /// <summary>
    /// 获取折旧汇总报表
    /// </summary>
    [HttpGet("depreciation-summary")]
    public async Task<ApiResult<List<object>>> DepreciationSummary([FromQuery] int year) => ApiResult<List<object>>.Success(await _service.GetDepreciationSummaryAsync(year));

    /// <summary>
    /// 获取资产价值统计
    /// </summary>
    [HttpGet("value-stats")]
    public async Task<ApiResult<object>> ValueStats() => ApiResult<object>.Success(await _service.GetValueStatsAsync());
}

/// <summary>资产处置控制器</summary>
[ApiController]
[Route("api/asset")]
public class AssetDisposeController : ControllerBase
{
    private readonly IAssetCardService _cardService;
    public AssetDisposeController(IAssetCardService cardService) => _cardService = cardService;

    /// <summary>资产处置（报废/出售）并生成凭证</summary>
    [HttpPost("card/{id}/dispose")]
    public async Task<ApiResult<long>> Dispose(long id, [FromBody] AssetDisposeRequest request)
    {
        return ApiResult<long>.Success(await _cardService.DisposeAsync(id, request, HttpContext.GetCurrentUserId()));
    }
}
