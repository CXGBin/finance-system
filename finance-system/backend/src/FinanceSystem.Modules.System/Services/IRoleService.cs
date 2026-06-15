using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 角色管理服务接口
/// </summary>
/// <summary>
/// 角色管理服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>角色分页数据</returns>
    Task<PageResult<SysRole>> GetPageAsync(RoleQuery query);

    /// <summary>
    /// 获取所有启用角色列表（用于下拉选择）
    /// </summary>
    /// <returns>角色列表</returns>
    Task<List<SysRole>> GetListAsync();

    /// <summary>
    /// 根据ID查询角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色详情（含菜单ID列表）</returns>
    Task<SysRole?> GetByIdAsync(long id);

    /// <summary>
    /// 新增角色
    /// </summary>
    /// <param name="request">新增请求</param>
    /// <returns>新角色ID</returns>
    Task<long> CreateAsync(RoleCreateRequest request);

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateAsync(long id, RoleCreateRequest request);

    /// <summary>
    /// 删除角色（校验关联用户）
    /// </summary>
    /// <param name="id">角色ID</param>
    Task DeleteAsync(long id);

    /// <summary>
    /// 获取角色已分配的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    Task<List<long>> GetRoleMenuIdsAsync(long roleId);
}
