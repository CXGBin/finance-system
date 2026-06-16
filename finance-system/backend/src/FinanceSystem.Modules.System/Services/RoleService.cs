using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 角色管理服务实现
/// </summary>
/// <summary>
/// 角色管理服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly ISqlSugarClient _db;

    public RoleService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<PageResult<SysRole>> GetPageAsync(RoleQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<SysRole>()
            .WhereIF(!string.IsNullOrEmpty(query.RoleName), r => r.RoleName.Contains(query.RoleName!))
            .WhereIF(!string.IsNullOrEmpty(query.RoleCode), r => r.RoleCode.Contains(query.RoleCode!))
            .WhereIF(query.Status.HasValue, r => r.Status == query.Status)
            .OrderBy(r => r.SortOrder)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<SysRole>(total, list);
    }

    /// <inheritdoc/>
    public async Task<List<SysRole>> GetListAsync()
    {
        return await _db.Queryable<SysRole>()
            .Where(r => r.Status == 1)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<SysRole?> GetByIdAsync(long id)
    {
        var role = await _db.Queryable<SysRole>().FirstAsync(r => r.Id == id);
        if (role != null)
        {
            role.MenuIds = await GetRoleMenuIdsAsync(id);
        }
        return role;
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(RoleCreateRequest request)
    {
        // 校验角色编码唯一
        var exists = await _db.Queryable<SysRole>().AnyAsync(r => r.RoleCode == request.RoleCode);
        if (exists)
            throw new BusinessException($"角色编码 '{request.RoleCode}' 已存在");

        var role = new SysRole
        {
            RoleName = request.RoleName,
            RoleCode = request.RoleCode,
            Description = request.Description,
            SortOrder = request.SortOrder,
            Status = request.Status
        };
        await _db.Insertable(role).ExecuteCommandAsync();

        // 保存角色菜单关联
        if (request.MenuIds.Any())
        {
            var roleMenus = request.MenuIds.Select(menuId => new SysRoleMenu
            {
                RoleId = role.Id,
                MenuId = menuId
            }).ToList();
            await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }

        return role.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, RoleCreateRequest request)
    {
        var role = await _db.Queryable<SysRole>().FirstAsync(r => r.Id == id)
            ?? throw new NotFoundException("角色不存在");

        role.RoleName = request.RoleName;
        role.RoleCode = request.RoleCode;
        role.Description = request.Description;
        role.SortOrder = request.SortOrder;
        role.Status = request.Status;
        await _db.Updateable(role).ExecuteCommandAsync();

        // 更新角色菜单关联：先删后插
        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();
        if (request.MenuIds.Any())
        {
            var roleMenus = request.MenuIds.Select(menuId => new SysRoleMenu
            {
                RoleId = id,
                MenuId = menuId
            }).ToList();
            await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        var role = await _db.Queryable<SysRole>().FirstAsync(r => r.Id == id)
            ?? throw new NotFoundException("角色不存在");

        // 校验预置角色（超级管理员）不可删除
        if (role.RoleCode == "SUPER_ADMIN")
            throw new BusinessException("超级管理员角色不可删除");

        // 校验是否有关联用户
        var hasUser = await _db.Queryable<SysUserRole>().AnyAsync(ur => ur.RoleId == id);
        if (hasUser)
            throw new BusinessException("该角色已分配给用户，请先取消关联后再删除");

        // 删除角色菜单关联
        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();
        // 删除角色
        await _db.Deleteable<SysRole>().Where(r => r.Id == id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<List<long>> GetRoleMenuIdsAsync(long roleId)
    {
        return await _db.Queryable<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task SaveRoleMenuIdsAsync(long roleId, List<long> menuIds)
    {
        var role = await _db.Queryable<SysRole>().FirstAsync(r => r.Id == roleId)
            ?? throw new NotFoundException("角色不存在");

        // 先删后插
        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == roleId).ExecuteCommandAsync();
        if (menuIds != null && menuIds.Any())
        {
            var roleMenus = menuIds.Select(menuId => new SysRoleMenu
            {
                RoleId = roleId,
                MenuId = menuId
            }).ToList();
            await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }
    }
}
