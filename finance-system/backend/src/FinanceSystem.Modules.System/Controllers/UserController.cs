using FinanceSystem.Core.Common;
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
        return ApiResult<PageResult<SysUser>>.Success(await _userService.GetPageAsync(query));
    }

    /// <summary>
    /// 查询用户详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<SysUser>> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null
            ? ApiResult<SysUser>.Fail("用户不存在")
            : ApiResult<SysUser>.Success(user);
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
    public async Task<ApiResult<SysUser>> GetProfile()
    {
        // TODO: 从JWT获取当前用户ID
        var user = await _userService.GetProfileAsync(0);
        return user == null
            ? ApiResult<SysUser>.Fail("用户不存在")
            : ApiResult<SysUser>.Success(user);
    }

    /// <summary>
    /// 修改个人信息
    /// </summary>
    [HttpPut("profile")]
    public async Task<ApiResult<bool>> UpdateProfile([FromBody] ProfileUpdateRequest request)
    {
        // TODO: 从JWT获取当前用户ID
        await _userService.UpdateProfileAsync(0, request);
        return ApiResult<bool>.Success(true);
    }
}
