using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/system/user")]
/// <summary>
/// 用户管理控制器
/// </summary>
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<PageResult<SysUser>>> GetPage([FromQuery] UserQuery query)
    {
        var result = await _userService.GetPageAsync(query);
        // 脱敏：清除密码哈希，不返回给前端
        foreach (var user in result.List) user.PasswordHash = null;
        return ApiResult<PageResult<SysUser>>.Success(result);
    }

    /// <summary>
    /// 查询用户详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<SysUser>> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return ApiResult<SysUser>.Fail("用户不存在");
        // 脱敏：清除密码哈希
        user.PasswordHash = null;
        return ApiResult<SysUser>.Success(user);
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] UserCreateRequest request)
    {
        return ApiResult<long>.Success(await _userService.CreateAsync(request));
    }

    /// <summary>
    /// 编辑用户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] UserCreateRequest request)
    {
        await _userService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _userService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 切换用户状态
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResult<bool>> ToggleStatus(long id, [FromQuery] int status)
    {
        await _userService.ToggleStatusAsync(id, status);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    [HttpPut("{id}/reset-password")]
    public async Task<ApiResult<bool>> ResetPassword(long id)
    {
        await _userService.ResetPasswordAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 获取个人信息
    /// </summary>
    [HttpGet("profile")]
    public async Task<ApiResult<UserProfile>> GetProfile()
    {
        var user = await _userService.GetProfileAsync(HttpContext.GetCurrentUserId());
        return user == null
            ? ApiResult<UserProfile>.Fail("用户不存在")
            : ApiResult<UserProfile>.Success(user);
    }

    /// <summary>
    /// 修改个人信息
    /// </summary>
    [HttpPut("profile")]
    public async Task<ApiResult<bool>> UpdateProfile([FromBody] ProfileUpdateRequest request)
    {
        await _userService.UpdateProfileAsync(HttpContext.GetCurrentUserId(), request);
        return ApiResult<bool>.Success(true);
    }
}
