using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Modules.Tax.Entities;
using FinanceSystem.Modules.Tax.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Tax.Controllers;

[ApiController]
[Route("api/tax/category")]
public class TaxCategoryController : ControllerBase
{
    private readonly ITaxCategoryService _service;
    public TaxCategoryController(ITaxCategoryService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<List<TaxCategory>>> GetList() => ApiResult<List<TaxCategory>>.Success(await _service.GetListAsync());

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] TaxCategoryRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] TaxCategoryRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/tax/declaration")]
public class TaxDeclarationController : ControllerBase
{
    private readonly ITaxDeclarationService _service;
    public TaxDeclarationController(ITaxDeclarationService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<PageResult<TaxDeclaration>>> GetList([FromQuery] TaxDeclarationQuery query) => ApiResult<PageResult<TaxDeclaration>>.Success(await _service.GetListAsync(query));

    [HttpPost("calculate")]
    public async Task<ApiResult<long>> Calculate([FromBody] TaxCalculateRequest request) => ApiResult<long>.Success(await _service.CalculateAsync(request, HttpContext.GetCurrentUserId()));

    [HttpPost("{id}/declare")]
    public async Task<ApiResult<bool>> Declare(long id) { await _service.DeclareAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/pay")]
    public async Task<ApiResult<bool>> Pay(long id) { await _service.ConfirmPayAsync(id); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 自动计算附加税
    /// </summary>
    [HttpPost("surcharges")]
    public async Task<ApiResult<List<TaxDeclaration>>> CalculateSurcharges([FromBody] SurchargeRequest request)
    {
        return ApiResult<List<TaxDeclaration>>.Success(await _service.CalculateSurchargesAsync(request.DeclarePeriod, request.VatDeclarationId, HttpContext.GetCurrentUserId()));
    }
}

[ApiController]
[Route("api/tax/invoice")]
public class TaxInvoiceController : ControllerBase
{
    private readonly ITaxInvoiceService _service;
    public TaxInvoiceController(ITaxInvoiceService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResult<PageResult<TaxInvoice>>> GetList([FromQuery] TaxInvoiceQuery query) => ApiResult<PageResult<TaxInvoice>>.Success(await _service.GetListAsync(query));

    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] TaxInvoiceRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    [HttpPost("{id}/verify")]
    public async Task<ApiResult<bool>> Verify(long id) { await _service.VerifyAsync(id); return ApiResult<bool>.Success(true); }
}

/// <summary>税务报表控制器</summary>
[ApiController]
[Route("api/tax/report")]
public class TaxReportController : ControllerBase
{
    private readonly ITaxReportService _service;
    public TaxReportController(ITaxReportService service) => _service = service;

    [HttpGet("summary")]
    public async Task<ApiResult<object>> Summary([FromQuery] int year) => ApiResult<object>.Success(await _service.GetSummaryAsync(year));

    [HttpGet("by-category")]
    public async Task<ApiResult<List<object>>> ByCategory([FromQuery] int year, [FromQuery] int? month) => ApiResult<List<object>>.Success(await _service.GetByCategoryAsync(year, month));

    /// <summary>税负率分析</summary>
    [HttpGet("burden")]
    public async Task<ApiResult<object>> Burden([FromQuery] int year, [FromQuery] int? quarter) => ApiResult<object>.Success(await _service.GetTaxBurdenAsync(year, quarter));
}

/// <summary>税务日历控制器</summary>
[ApiController]
[Route("api/tax/calendar")]
public class TaxCalendarController : ControllerBase
{
    private readonly ITaxCalendarService _service;
    public TaxCalendarController(ITaxCalendarService service) => _service = service;

    [HttpGet]
    public async Task<ApiResult<List<object>>> Calendar([FromQuery] int year, [FromQuery] int month) => ApiResult<List<object>>.Success(await _service.GetCalendarAsync(year, month));
}
