using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/system/role")]
/// <summary>
/// 角色管理控制器
/// </summary>
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<PageResult<SysRole>>> GetPage([FromQuery] RoleQuery query)
    {
        return ApiResult<PageResult<SysRole>>.Success(await _roleService.GetPageAsync(query));
    }

    /// <summary>
    /// 获取所有启用角色列表（下拉）
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<SysRole>>> GetList()
    {
        return ApiResult<List<SysRole>>.Success(await _roleService.GetListAsync());
    }

    /// <summary>
    /// 查询角色详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<SysRole>> GetById(long id)
    {
        var role = await _roleService.GetByIdAsync(id);
        return role == null
            ? ApiResult<SysRole>.Fail("角色不存在")
            : ApiResult<SysRole>.Success(role);
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] RoleCreateRequest request)
    {
        return ApiResult<long>.Success(await _roleService.CreateAsync(request));
    }

    /// <summary>
    /// 编辑角色
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] RoleCreateRequest request)
    {
        await _roleService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _roleService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 获取角色已分配菜单
    /// </summary>
    [HttpGet("{id}/menus")]
    public async Task<ApiResult<List<long>>> GetRoleMenus(long id)
    {
        return ApiResult<List<long>>.Success(await _roleService.GetRoleMenuIdsAsync(id));
    }
}
