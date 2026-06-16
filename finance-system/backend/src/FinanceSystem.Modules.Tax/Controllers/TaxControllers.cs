using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Tax.DTOs;
using FinanceSystem.Modules.Tax.Entities;
using FinanceSystem.Modules.Tax.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Tax.Controllers;

[ApiController]
[Route("api/tax/category")]
/// <summary>
/// 税种配置控制器
/// </summary>
public class TaxCategoryController : ControllerBase
{
    private readonly ITaxCategoryService _service;
    public TaxCategoryController(ITaxCategoryService service) => _service = service;

    /// <summary>
    /// 获取税种列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<TaxCategory>>> GetList() => ApiResult<List<TaxCategory>>.Success(await _service.GetListAsync());

    /// <summary>
    /// 新增税种
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] TaxCategoryRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    /// <summary>
    /// 修改税种
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] TaxCategoryRequest request) { await _service.UpdateAsync(id, request); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 删除税种
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }
}

[ApiController]
[Route("api/tax/declaration")]
/// <summary>
/// 纳税申报控制器
/// </summary>
public class TaxDeclarationController : ControllerBase
{
    private readonly ITaxDeclarationService _service;
    public TaxDeclarationController(ITaxDeclarationService service) => _service = service;

    /// <summary>
    /// 分页查询纳税申报列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<TaxDeclaration>>> GetList([FromQuery] TaxDeclarationQuery query) => ApiResult<PageResult<TaxDeclaration>>.Success(await _service.GetListAsync(query));

    /// <summary>
    /// 计算税额并生成申报记录
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ApiResult<long>> Calculate([FromBody] TaxCalculateRequest request) => ApiResult<long>.Success(await _service.CalculateAsync(request, HttpContext.GetCurrentUserId()));

    /// <summary>
    /// 提交纳税申报
    /// </summary>
    [HttpPost("{id}/declare")]
    public async Task<ApiResult<bool>> Declare(long id) { await _service.DeclareAsync(id, HttpContext.GetCurrentUserId()); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 确认缴款
    /// </summary>
    [HttpPost("{id}/pay")]
    public async Task<ApiResult<bool>> Pay(long id) { await _service.ConfirmPayAsync(id); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 自动计算附加税（城建税+教育费附加+地方教育附加）
    /// </summary>
    [HttpPost("surcharges")]
    public async Task<ApiResult<List<TaxDeclaration>>> CalculateSurcharges([FromBody] SurchargeRequest request)
    {
        return ApiResult<List<TaxDeclaration>>.Success(await _service.CalculateSurchargesAsync(request.DeclarePeriod, request.VatDeclarationId, HttpContext.GetCurrentUserId()));
    }
}

[ApiController]
[Route("api/tax/invoice")]
/// <summary>
/// 发票管理控制器
/// </summary>
public class TaxInvoiceController : ControllerBase
{
    private readonly ITaxInvoiceService _service;
    public TaxInvoiceController(ITaxInvoiceService service) => _service = service;

    /// <summary>
    /// 分页查询发票列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PageResult<TaxInvoice>>> GetList([FromQuery] TaxInvoiceQuery query) => ApiResult<PageResult<TaxInvoice>>.Success(await _service.GetListAsync(query));

    /// <summary>
    /// 新增发票
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] TaxInvoiceRequest request) => ApiResult<long>.Success(await _service.CreateAsync(request));

    /// <summary>
    /// 删除发票
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id) { await _service.DeleteAsync(id); return ApiResult<bool>.Success(true); }

    /// <summary>
    /// 验真发票
    /// </summary>
    [HttpPost("{id}/verify")]
    public async Task<ApiResult<bool>> Verify(long id) { await _service.VerifyAsync(id); return ApiResult<bool>.Success(true); }
}

/// <summary>税务报表控制器</summary>
[ApiController]
[Route("api/tax/report")]
/// <summary>
/// 税务报表控制器
/// </summary>
public class TaxReportController : ControllerBase
{
    private readonly ITaxReportService _service;
    public TaxReportController(ITaxReportService service) => _service = service;

    /// <summary>
    /// 获取税务汇总报表
    /// </summary>
    [HttpGet("summary")]
    public async Task<ApiResult<object>> Summary([FromQuery] int year) => ApiResult<object>.Success(await _service.GetSummaryAsync(year));

    /// <summary>
    /// 按税种分类查询申报汇总
    /// </summary>
    [HttpGet("by-category")]
    public async Task<ApiResult<List<object>>> ByCategory([FromQuery] int year, [FromQuery] int? month) => ApiResult<List<object>>.Success(await _service.GetByCategoryAsync(year, month));

    /// <summary>税负率分析</summary>
    [HttpGet("burden")]
    public async Task<ApiResult<object>> Burden([FromQuery] int year, [FromQuery] int? quarter) => ApiResult<object>.Success(await _service.GetTaxBurdenAsync(year, quarter));
}

/// <summary>税务日历控制器</summary>
[ApiController]
[Route("api/tax/calendar")]
/// <summary>
/// 税务日历控制器
/// </summary>
public class TaxCalendarController : ControllerBase
{
    private readonly ITaxCalendarService _service;
    public TaxCalendarController(ITaxCalendarService service) => _service = service;

    /// <summary>
    /// 获取指定月份税务日历事项
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<List<object>>> Calendar([FromQuery] int year, [FromQuery] int month) => ApiResult<List<object>>.Success(await _service.GetCalendarAsync(year, month));
}
