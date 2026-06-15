using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 用户管理服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly ISqlSugarClient _db;
    private const string DefaultPassword = "123456";

    public UserService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<PageResult<SysUser>> GetPageAsync(UserQuery query)
    {
        var queryable = _db.Queryable<SysUser>()
            .WhereIF(!string.IsNullOrEmpty(query.Username), u => u.Username.Contains(query.Username!))
            .WhereIF(!string.IsNullOrEmpty(query.RealName), u => u.RealName.Contains(query.RealName!))
            .WhereIF(!string.IsNullOrEmpty(query.Phone), u => u.Phone != null && u.Phone.Contains(query.Phone!))
            .WhereIF(query.Status.HasValue, u => u.Status == query.Status)
            .WhereIF(query.DeptId.HasValue, u => u.DeptId == query.DeptId)
            .WhereIF(query.StartTime.HasValue, u => u.CreatedTime >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, u => u.CreatedTime <= query.EndTime);

        // 按角色筛选需要子查询
        if (query.RoleId.HasValue)
        {
            var userIds = await _db.Queryable<SysUserRole>()
                .Where(ur => ur.RoleId == query.RoleId.Value)
                .Select(ur => ur.UserId)
                .ToListAsync();
            queryable = queryable.Where(u => userIds.Contains(u.Id));
        }

        RefAsync<int> total = 0;
        var list = await queryable
            .OrderBy(u => u.CreatedTime, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<SysUser>(total, list);
    }

    /// <inheritdoc/>
    public async Task<SysUser?> GetByIdAsync(long id)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id);
        if (user != null)
        {
            // 查询关联角色
            var roleIds = await _db.Queryable<SysUserRole>()
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();
            user.Roles = await _db.Queryable<SysRole>()
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();
        }
        return user;
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(UserCreateRequest request)
    {
        // 校验用户名唯一
        var exists = await _db.Queryable<SysUser>().AnyAsync(u => u.Username == request.Username);
        if (exists)
            throw new BusinessException($"用户名 '{request.Username}' 已存在");

        // 校验角色ID有效性
        if (request.RoleIds.Any())
        {
            var validRoleCount = await _db.Queryable<SysRole>()
                .Where(r => request.RoleIds.Contains(r.Id))
                .CountAsync();
            if (validRoleCount != request.RoleIds.Count)
                throw new BusinessException("包含无效的角色ID");
        }

        // 创建用户
        var user = new SysUser
        {
            Username = request.Username!,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password ?? DefaultPassword),
            RealName = request.RealName,
            Email = request.Email,
            Phone = request.Phone,
            Avatar = request.Avatar,
            DeptId = request.DeptId,
            PostId = request.PostId,
            Status = request.Status,
            Remark = request.Remark
        };
        await _db.Insertable(user).ExecuteCommandAsync();

        // 保存用户角色关联
        if (request.RoleIds.Any())
        {
            var userRoles = request.RoleIds.Select(roleId => new SysUserRole
            {
                UserId = user.Id,
                RoleId = roleId
            }).ToList();
            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return user.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, UserCreateRequest request)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id)
            ?? throw new NotFoundException("用户不存在");

        // 编辑时不允许修改用户名
        user.RealName = request.RealName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Avatar = request.Avatar;
        user.DeptId = request.DeptId;
        user.PostId = request.PostId;
        user.Status = request.Status;
        user.Remark = request.Remark;
        user.UpdatedTime = DateTime.Now;

        // 如果提供了新密码则更新
        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _db.Updateable(user).ExecuteCommandAsync();

        // 更新角色关联：先删后插
        await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == id).ExecuteCommandAsync();
        if (request.RoleIds.Any())
        {
            var userRoles = request.RoleIds.Select(roleId => new SysUserRole
            {
                UserId = id,
                RoleId = roleId
            }).ToList();
            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id)
            ?? throw new NotFoundException("用户不存在");

        // 超级管理员不可删除（username=admin）
        if (user.Username == "admin")
            throw new BusinessException("超级管理员不可删除");

        // 校验是否有关联日志
        var hasLog = await _db.Queryable<SysLog>().AnyAsync(l => l.UserId == id);
        if (hasLog)
            throw new BusinessException("该用户已有关联操作日志，仅支持停用，不可删除");

        // 删除用户角色关联
        await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == id).ExecuteCommandAsync();
        // 删除用户
        await _db.Deleteable<SysUser>().Where(u => u.Id == id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task ToggleStatusAsync(long id, int status)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id)
            ?? throw new NotFoundException("用户不存在");

        if (user.Username == "admin")
            throw new BusinessException("超级管理员不可停用");

        user.Status = status;
        await _db.Updateable(user).UpdateColumns(u => new { u.Status, u.UpdatedTime }).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task ResetPasswordAsync(long id)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id)
            ?? throw new NotFoundException("用户不存在");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
        await _db.Updateable(user).UpdateColumns(u => new { u.PasswordHash, u.UpdatedTime }).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<SysUser?> GetProfileAsync(long userId)
    {
        return await _db.Queryable<SysUser>().FirstAsync(u => u.Id == userId);
    }

    /// <inheritdoc/>
    public async Task UpdateProfileAsync(long userId, ProfileUpdateRequest request)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == userId)
            ?? throw new NotFoundException("用户不存在");

        if (!string.IsNullOrEmpty(request.RealName)) user.RealName = request.RealName;
        if (request.Email != null) user.Email = request.Email;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Avatar != null) user.Avatar = request.Avatar;

        user.UpdatedTime = DateTime.Now;
        await _db.Updateable(user).UpdateColumns(u => new
        {
            u.RealName, u.Email, u.Phone, u.Avatar, u.UpdatedTime
        }).ExecuteCommandAsync();
    }
}
