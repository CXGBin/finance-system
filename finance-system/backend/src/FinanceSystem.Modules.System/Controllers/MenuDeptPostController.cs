using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/system/menu")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取完整菜单树
    /// </summary>
    [HttpGet("tree")]
    public async Task<ApiResult<List<SysMenu>>> GetTree()
    {
        return ApiResult<List<SysMenu>>.Success(await _menuService.GetTreeAsync());
    }

    /// <summary>
    /// 新增菜单
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] MenuCreateRequest request)
    {
        return ApiResult<long>.Success(await _menuService.CreateAsync(request));
    }

    /// <summary>
    /// 编辑菜单
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] MenuCreateRequest request)
    {
        await _menuService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除菜单（级联删除子菜单）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _menuService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("api/system/dept")]
public class DeptController : ControllerBase
{
    private readonly IDeptService _deptService;

    public DeptController(IDeptService deptService)
    {
        _deptService = deptService;
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    [HttpGet("tree")]
    public async Task<ApiResult<List<SysDept>>> GetTree()
    {
        return ApiResult<List<SysDept>>.Success(await _deptService.GetTreeAsync());
    }

    /// <summary>
    /// 新增部门
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] DeptCreateRequest request)
    {
        return ApiResult<long>.Success(await _deptService.CreateAsync(request));
    }

    /// <summary>
    /// 编辑部门
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] DeptCreateRequest request)
    {
        await _deptService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _deptService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 岗位管理控制器
/// </summary>
[ApiController]
[Route("api/system/post")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// 分页查询岗位列表
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<Core.Common.PageResult<SysPost>>> GetPage(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? deptId = null,
        [FromQuery] string? postName = null)
    {
        return ApiResult<Core.Common.PageResult<SysPost>>.Success(
            await _postService.GetPageAsync(pageIndex, pageSize, deptId, postName));
    }

    /// <summary>
    /// 新增岗位
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<long>> Create([FromBody] PostCreateRequest request)
    {
        return ApiResult<long>.Success(await _postService.CreateAsync(request));
    }

    /// <summary>
    /// 编辑岗位
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResult<bool>> Update(long id, [FromBody] PostCreateRequest request)
    {
        await _postService.UpdateAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResult<bool>> Delete(long id)
    {
        await _postService.DeleteAsync(id);
        return ApiResult<bool>.Success(true);
    }
}
