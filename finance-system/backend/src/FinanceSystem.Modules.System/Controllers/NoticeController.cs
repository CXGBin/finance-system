using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 系统公告控制器
/// </summary>
[ApiController]
[Route("api/system/notice")]
/// <summary>
/// 系统公告控制器
/// </summary>
public class NoticeController : ControllerBase
{
    private readonly INoticeService _noticeService;

    public NoticeController(INoticeService noticeService) => _noticeService = noticeService;

    /// <summary>
    /// 获取公告列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<SysNotice>>> GetList([FromQuery] int? noticeType = null)
    {
        return ApiResult<List<SysNotice>>.Success(await _noticeService.GetListAsync(noticeType));
    }

    /// <summary>
    /// 新增公告
    /// </summary>
    [HttpPost("")]
    public async Task<ApiResult<long>> Create([FromBody] NoticeCreateRequest request)
    {
        return ApiResult<long>.Success(await _noticeService.CreateAsync(request, HttpContext.GetCurrentUserId()));
    }

    /// <summary>
    /// 修改公告
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] NoticeCreateRequest request)
    {
        await _noticeService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _noticeService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}
