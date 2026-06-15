using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Reports.DTOs;
using FinanceSystem.Modules.Reports.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Reports.Controllers;

/// <summary>
/// 资产负债表控制器
/// </summary>
[ApiController]
[Route("api/report/balance-sheet")]
public class BalanceSheetController : ControllerBase
{
    private readonly IBalanceSheetService _service;

    public BalanceSheetController(IBalanceSheetService service) => _service = service;

    /// <summary>
    /// 查询资产负债表
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<BalanceSheetResult>> Get([FromQuery] ReportQuery query)
    {
        return ApiResult<BalanceSheetResult>.Success(await _service.GenerateAsync(query));
    }
}

/// <summary>
/// 利润表控制器
/// </summary>
[ApiController]
[Route("api/report/income-statement")]
public class IncomeStatementController : ControllerBase
{
    private readonly IIncomeStatementService _service;

    public IncomeStatementController(IIncomeStatementService service) => _service = service;

    /// <summary>
    /// 查询利润表
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<IncomeStatementResult>> Get([FromQuery] IncomeStatementQuery query)
    {
        return ApiResult<IncomeStatementResult>.Success(await _service.GenerateAsync(query));
    }
}

/// <summary>
/// 现金流量表控制器
/// </summary>
[ApiController]
[Route("api/report/cash-flow")]
public class CashFlowController : ControllerBase
{
    private readonly ICashFlowService _service;

    public CashFlowController(ICashFlowService service) => _service = service;

    /// <summary>
    /// 查询现金流量表
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<CashFlowResult>> Get([FromQuery] ReportQuery query)
    {
        return ApiResult<CashFlowResult>.Success(await _service.GenerateAsync(query));
    }
}

/// <summary>
/// 科目余额表控制器
/// </summary>
[ApiController]
[Route("api/report/subject-balance")]
public class SubjectBalanceReportController : ControllerBase
{
    private readonly ISubjectBalanceReportService _service;

    public SubjectBalanceReportController(ISubjectBalanceReportService service) => _service = service;

    /// <summary>
    /// 查询科目余额表
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<object>> Get([FromQuery] SubjectBalanceReportQuery query)
    {
        return ApiResult<object>.Success(await _service.GetReportAsync(query));
    }
}

/// <summary>
/// 自定义报表控制器
/// </summary>
[ApiController]
[Route("api/report/custom")]
public class CustomReportController : ControllerBase
{
    private readonly ICustomReportService _service;

    public CustomReportController(ICustomReportService service) => _service = service;

    /// <summary>
    /// 获取模板列表
    /// </summary>
    [HttpGet("template/list")]
    public async Task<ApiResult<PageResult<object>>> GetTemplates([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return ApiResult<PageResult<object>>.Success(await _service.GetTemplatesAsync(pageIndex, pageSize));
    }

    /// <summary>
    /// 创建模板
    /// </summary>
    [HttpPost("template")]
    public async Task<ApiResult<long>> Create([FromBody] ReportTemplateRequest request)
    {
        return ApiResult<long>.Success(await _service.CreateTemplateAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 修改模板
    /// </summary>
    [HttpPut("template/{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] ReportTemplateRequest request)
    {
        await _service.UpdateTemplateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除模板
    /// </summary>
    [HttpDelete("template/{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _service.DeleteTemplateAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 按模板生成报表
    /// </summary>
    [HttpGet("template/{id}/generate")]
    public async Task<ApiResult<object>> Generate(long id, [FromQuery] string period)
    {
        return ApiResult<object>.Success(await _service.GenerateCustomReportAsync(id, period));
    }
}

/// <summary>
/// 报表导出控制器
/// </summary>
[ApiController]
[Route("api/report/export")]
public class ReportExportController : ControllerBase
{
    private readonly IReportExportService _service;

    public ReportExportController(IReportExportService service) => _service = service;

    /// <summary>
    /// 导出报表
    /// </summary>
    [HttpGet("excel")]
    public async Task<ApiResult<string>> ExportExcel([FromQuery] ExportQuery query)
    {
        query.Format = "excel";
        return ApiResult<string>.Success(await _service.ExportAsync(query));
    }

    /// <summary>
    /// 导出报表（PDF）
    /// </summary>
    [HttpGet("pdf")]
    public async Task<ApiResult<string>> ExportPdf([FromQuery] ExportQuery query)
    {
        query.Format = "pdf";
        return ApiResult<string>.Success(await _service.ExportAsync(query));
    }
}

/// <summary>
/// 多期对比控制器
/// </summary>
[ApiController]
[Route("api/report/compare")]
public class CompareController : ControllerBase
{
    private readonly ICompareService _service;

    public CompareController(ICompareService service) => _service = service;

    /// <summary>
    /// 多期对比分析
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<CompareResult>> Get([FromQuery] CompareQuery query)
    {
        return ApiResult<CompareResult>.Success(await _service.CompareAsync(query));
    }
}
