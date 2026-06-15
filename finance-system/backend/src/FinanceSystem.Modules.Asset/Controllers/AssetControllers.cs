using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Asset.DTOs;
using FinanceSystem.Modules.Asset.Entities;
using FinanceSystem.Modules.Asset.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Asset.Controllers;

[ApiController]
[Route("api/asset/category")]
public class AssetCategoryController : ControllerBase
{
    private readonly IAssetCategoryService _service;
    public AssetCategoryController(IAssetCategoryService service) => _service = service;

    [HttpGet("tree")]
    public async Task<ApiResult<List<AssetCategory>>> GetTree() => ApiResult<List<AssetCategory>>.Success(await _service.GetTreeAsync());

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] AssetCategoryRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] AssetCategoryRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/asset/card")]
public class AssetCardController : ControllerBase
{
    private readonly IAssetCardService _service;
    public AssetCardController(IAssetCardService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResult<PageResult<AssetCard>>> GetPage([FromQuery] AssetCardQuery query) => ApiResult<PageResult<AssetCard>>.Success(await _service.GetPageAsync(query));

    [HttpGet("{id}")]
    public async Task<ApiResult<AssetCard>> GetById(long id) { var r = await _service.GetByIdAsync(id); return r == null ? ApiResult<AssetCard>.Fail("不存在") : ApiResult<AssetCard>.Success(r); }

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] AssetCardRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] AssetCardRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/change")]
    public async Task<ApiResult<bool>> ChangeStatus(long id, [FromQuery] int changeType, [FromBody] AssetChangeRequest request) { await _service.ChangeStatusAsync(id, changeType, request, 0); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/asset/depreciation")]
public class AssetDepreciationController : ControllerBase
{
    private readonly IAssetDepreciationService _service;
    public AssetDepreciationController(IAssetDepreciationService service) => _service = service;

    [HttpGet("calculate")]
    public async Task<ApiResult<List<AssetDepreciation>>> Calculate([FromQuery] int year, [FromQuery] int month) => ApiResult<List<AssetDepreciation>>.Success(await _service.CalculateAsync(year, month));

    [HttpPost("confirm")]
    public async Task<ApiResult<bool>> Confirm([FromQuery] int year, [FromQuery] int month) { await _service.ConfirmDepreciationAsync(year, month, 0); return ApiResult<bool>.Success(true); }

    [HttpGet("summary")]
    public async Task<ApiResult<List<object>>> GetSummary([FromQuery] int year) => ApiResult<List<object>>.Success(await _service.GetSummaryAsync(year));
}
