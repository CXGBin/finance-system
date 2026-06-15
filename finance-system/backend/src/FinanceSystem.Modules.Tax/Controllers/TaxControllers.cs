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
    public async Task<ApiResult<long>> Calculate([FromBody] TaxCalculateRequest request) => ApiResult<long>.Success(await _service.CalculateAsync(request));

    [HttpPost("{id}/declare")]
    public async Task<ApiResult<bool>> Declare(long id) { await _service.DeclareAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    [HttpPost("{id}/pay")]
    public async Task<ApiResult<bool>> Pay(long id) { await _service.ConfirmPayAsync(id); return ApiResult<bool>.Success(true); }
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
