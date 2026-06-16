using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Accounts.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.Accounts.Controllers;

/// <summary>
/// 会计科目管理控制器
/// </summary>
[ApiController]
[Route("api/account/subject")]
/// <summary>
/// 会计科目控制器
/// </summary>
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    /// <summary>
    /// 获取科目树（仅启用）
    /// </summary>
    [HttpGet("tree")]
    public async Task<ApiResult<List<AccountSubject>>> GetTree([FromQuery] bool enabledOnly = true)
    {
        return ApiResult<List<AccountSubject>>.Success(await _subjectService.GetTreeAsync(enabledOnly));
    }

    /// <summary>
    /// 获取科目详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<AccountSubject>> GetById(long id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        return subject == null
            ? ApiResult<AccountSubject>.Fail("科目不存在")
            : ApiResult<AccountSubject>.Success(subject);
    }

    /// <summary>
    /// 新增科目
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] SubjectCreateRequest request)
    {
        return ApiResult<long>.Success(await _subjectService.CreateAsync(request));
    }

    /// <summary>
    /// 修改科目
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] SubjectCreateRequest request)
    {
        await _subjectService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除科目
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _subjectService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 启用/停用科目
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResult<bool>> ToggleStatus(long id, [FromQuery] int isEnabled)
    {
        await _subjectService.ToggleStatusAsync(id, isEnabled);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 批量导入科目（接收JSON数组格式的科目列表）
    /// </summary>
    [HttpPost("import")]
    public async Task<ApiResult<int>> Import([FromBody] List<SubjectCreateRequest> subjects)
    {
        if (subjects == null || !subjects.Any()) throw new BusinessException("导入数据为空");
        var count = 0;
        foreach (var item in subjects)
        {
            try
            {
                await _subjectService.CreateAsync(item);
                count++;
            }
            catch
            {
                // 跳过已存在的科目，继续导入
            }
        }
        return ApiResult<int>.Success(count);
    }

    /// <summary>
    /// 导出全部科目（返回JSON数组，供前端下载）
    /// </summary>
    [HttpGet("export")]
    public async Task<ApiResult<List<object>>> Export()
    {
        var tree = await _subjectService.GetTreeAsync(false);
        // 扁平化树结构方便导出
        var result = new List<object>();
        foreach (var item in tree) result.Add(new { item.SubjectCode, item.SubjectName, item.SubjectType, item.BalanceDirection, item.IsEnabled });
        return ApiResult<List<object>>.Success(result);
    }
}

/// <summary>
/// 凭证管理控制器
/// </summary>
[ApiController]
[Route("api/account/voucher")]
/// <summary>
/// 凭证控制器
/// </summary>
public class VoucherController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VoucherController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// 分页查询凭证
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<PageResult<Voucher>>> GetPage([FromQuery] VoucherQuery query)
    {
        return ApiResult<PageResult<Voucher>>.Success(await _voucherService.GetPageAsync(query));
    }

    /// <summary>
    /// 获取凭证详情（含分录）
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<Voucher>> GetById(long id)
    {
        var voucher = await _voucherService.GetByIdAsync(id);
        return voucher == null
            ? ApiResult<Voucher>.Fail("凭证不存在")
            : ApiResult<Voucher>.Success(voucher);
    }

    /// <summary>
    /// 新增凭证（草稿）
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] VoucherCreateRequest request)
    {
        return ApiResult<long>.Success(await _voucherService.CreateAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 删除凭证（仅草稿状态）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _voucherService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 修改凭证（仅草稿）
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] VoucherCreateRequest request)
    {
        await _voucherService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 审核凭证
    /// </summary>
    [HttpPost("{id}/audit")]
    public async Task<ApiResult<bool>> Audit(long id)
    {
        await _voucherService.AuditAsync(id, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 反审核凭证
    /// </summary>
    [HttpPost("{id}/unaudit")]
    public async Task<ApiResult<bool>> UnAudit(long id)
    {
        await _voucherService.UnAuditAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 作废凭证
    /// </summary>
    [HttpPost("{id}/void")]
    public async Task<ApiResult<bool>> Void(long id)
    {
        await _voucherService.VoidAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 批量审核凭证
    /// </summary>
    [HttpPost("batch-audit")]
    public async Task<ApiResult<bool>> BatchAudit([FromBody] List<long> ids)
    {
        await _voucherService.BatchAuditAsync(ids, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 批量作废凭证
    /// </summary>
    [HttpPost("batch-void")]
    public async Task<ApiResult<bool>> BatchVoid([FromBody] List<long> ids)
    {
        await _voucherService.BatchVoidAsync(ids);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 红字冲销凭证
    /// </summary>
    [HttpPost("{id}/reverse")]
    public async Task<ApiResult<long>> Reverse(long id)
    {
        return ApiResult<long>.Success(await _voucherService.ReverseAsync(id, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 获取凭证打印数据（返回凭证详情+分录，供前端渲染PDF/打印）
    /// </summary>
    [HttpGet("{id}/print-data")]
    public async Task<ApiResult<object>> GetPrintData(long id)
    {
        var voucher = await _voucherService.GetByIdAsync(id)
            ?? throw new BusinessException("凭证不存在");
        return ApiResult<object>.Success(voucher);
    }

    /// <summary>
    /// 复制凭证（生成草稿副本，清除审核信息）
    /// </summary>
    [HttpPost("{id}/copy")]
    public async Task<ApiResult<long>> Copy(long id)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        var newId = await _voucherService.CopyAsync(id, currentUserId);
        return ApiResult<long>.Success(newId);
    }
}

/// <summary>
/// 会计期间管理控制器
/// </summary>
[ApiController]
[Route("api/account/period")]
/// <summary>
/// 会计期间控制器
/// </summary>
public class PeriodController : ControllerBase
{
    private readonly IPeriodService _periodService;

    public PeriodController(IPeriodService periodService)
    {
        _periodService = periodService;
    }

    /// <summary>
    /// 获取年度期间列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<AccountingPeriod>>> GetListByYear([FromQuery] int year)
    {
        return ApiResult<List<AccountingPeriod>>.Success(await _periodService.GetListByYearAsync(year));
    }

    /// <summary>
    /// 获取当前期间
    /// </summary>
    [HttpGet("current")]
    public async Task<ApiResult<AccountingPeriod>> GetCurrent()
    {
        var period = await _periodService.GetCurrentAsync();
        return period == null
            ? ApiResult<AccountingPeriod>.Fail("无当前期间")
            : ApiResult<AccountingPeriod>.Success(period);
    }

    /// <summary>
    /// 初始化年度期间
    /// </summary>
    [HttpPost("init-year")]
    public async Task<ApiResult<bool>> InitYear([FromQuery] int year)
    {
        await _periodService.InitYearAsync(year);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 期末结账
    /// </summary>
    [HttpPost("close")]
    public async Task<ApiResult<bool>> Close([FromQuery] long periodId)
    {
        await _periodService.CloseAsync(periodId, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 反结账
    /// </summary>
    [HttpPost("unclose")]
    public async Task<ApiResult<bool>> UnClose([FromQuery] long periodId)
    {
        await _periodService.UnCloseAsync(periodId);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 期末损益结转
    /// </summary>
    [HttpPost("profit-transfer")]
    public async Task<ApiResult<bool>> ProfitTransfer([FromQuery] long periodId)
    {
        await _periodService.ProfitTransferAsync(periodId, HttpContext.GetCurrentUserId());
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 科目期初余额控制器
/// </summary>
[ApiController]
[Route("api/account/subject/balance")]
/// <summary>
/// 科目余额控制器
/// </summary>
public class SubjectBalanceController : ControllerBase
{
    private readonly ISubjectBalanceService _balanceService;

    public SubjectBalanceController(ISubjectBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    /// <summary>
    /// 查询某期间期初余额
    /// </summary>
    [HttpGet("{periodId}")]
    public async Task<ApiResult<List<SubjectBalance>>> GetByPeriod(long periodId)
    {
        return ApiResult<List<SubjectBalance>>.Success(await _balanceService.GetByPeriodAsync(periodId));
    }

    /// <summary>
    /// 批量保存期初余额
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<bool>> Save([FromBody] List<SubjectBalanceRequest> items)
    {
        await _balanceService.SaveAsync(items);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 试算平衡校验
    /// </summary>
    [HttpGet("trial-balance")]
    public async Task<ApiResult<object>> TrialBalance([FromQuery] long periodId)
    {
        var result = await _balanceService.TrialBalanceAsync(periodId);
        return ApiResult<object>.Success(new
        {
            result.IsBalanced,
            result.DebitTotal,
            result.CreditTotal
        });
    }
}

/// <summary>
/// 账簿查询控制器
/// </summary>
[ApiController]
[Route("api/account/ledger")]
/// <summary>
/// 账簿查询控制器
/// </summary>
public class LedgerController : ControllerBase
{
    private readonly ILedgerService _ledgerService;

    public LedgerController(ILedgerService ledgerService)
    {
        _ledgerService = ledgerService;
    }

    /// <summary>
    /// 总账查询
    /// </summary>
    [HttpGet("general")]
    public async Task<ApiResult<List<SubjectBalance>>> GetGeneralLedger([FromQuery] LedgerQuery query)
    {
        return ApiResult<List<SubjectBalance>>.Success(await _ledgerService.GetGeneralLedgerAsync(query));
    }

    /// <summary>
    /// 明细账查询
    /// </summary>
    [HttpGet("detail")]
    public async Task<ApiResult<PageResult<object>>> GetDetailLedger([FromQuery] LedgerQuery query)
    {
        return ApiResult<PageResult<object>>.Success(await _ledgerService.GetDetailLedgerAsync(query));
    }

    /// <summary>
    /// 日记账查询（现金/银行）
    /// </summary>
    [HttpGet("journal")]
    public async Task<ApiResult<PageResult<object>>> GetJournal([FromQuery] LedgerQuery query)
    {
        return ApiResult<PageResult<object>>.Success(await _ledgerService.GetJournalAsync(query));
    }

    /// <summary>
    /// 科目汇总表
    /// </summary>
    [HttpGet("subject-summary")]
    public async Task<ApiResult<List<SubjectBalance>>> GetSubjectSummary([FromQuery] int year, [FromQuery] int month)
    {
        return ApiResult<List<SubjectBalance>>.Success(await _ledgerService.GetSubjectSummaryAsync(year, month));
    }
}

/// <summary>
/// 辅助核算管理控制器
/// </summary>
[ApiController]
[Route("api/account/auxiliary/{type}")]
/// <summary>
/// 辅助核算控制器
/// </summary>
public class AuxiliaryController : ControllerBase
{
    private readonly IAuxiliaryService _auxiliaryService;

    public AuxiliaryController(IAuxiliaryService auxiliaryService)
    {
        _auxiliaryService = auxiliaryService;
    }

    /// <summary>
    /// 获取辅助核算项列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<object>> GetList(string type, [FromQuery] AuxiliaryQuery query)
    {
        return ApiResult<object>.Success(await _auxiliaryService.GetListAsync(type, query));
    }

    /// <summary>
    /// 新增辅助核算项
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create(string type, [FromBody] dynamic request)
    {
        return ApiResult<long>.Success(await _auxiliaryService.CreateAsync(type, request));
    }

    /// <summary>
    /// 修改辅助核算项
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(string type, long id, [FromBody] dynamic request)
    {
        await _auxiliaryService.UpdateAsync(type, id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除辅助核算项
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(string type, long id)
    {
        await _auxiliaryService.DeleteAsync(type, id);
        return ApiResult<bool>.Success(true);
    }
}
