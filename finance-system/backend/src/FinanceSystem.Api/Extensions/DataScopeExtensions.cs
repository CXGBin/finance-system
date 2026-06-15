using FinanceSystem.Modules.System.Entities;
using SqlSugar;

namespace FinanceSystem.Api.Extensions;

/// <summary>
/// 数据权限过滤扩展：基于角色的DataScope控制数据可见范围
/// DataScope: 1=全部数据 2=本部门 3=本部门及子部门 4=仅本人
/// </summary>
public static class DataScopeExtensions
{
    /// <summary>
    /// 获取当前用户数据权限过滤的部门ID列表
    /// 返回null表示不过滤，空数组表示仅本人数据
    /// </summary>
    public static async Task<long[]?> GetDataScopeDeptIdsAsync(this ISqlSugarClient db, long userId)
    {
        if (userId == 0) return null;

        var user = await db.Queryable<SysUser>().FirstAsync(u => u.Id == userId);
        if (user == null) return null;

        var roleIds = await db.Queryable<SysUserRole>().Where(r => r.UserId == userId).Select(r => r.RoleId).ToListAsync();
        if (!roleIds.Any()) return null;

        var maxScope = await db.Queryable<SysRole>().Where(r => roleIds.Contains(r.Id)).MaxAsync(r => r.DataScope);

        switch (maxScope)
        {
            case 1: return null; // 全部数据
            case 2: return new[] { user.DeptId ?? 0 }; // 本部门
            case 3:
                var childDepts = await db.Queryable<SysDept>().Where(d => d.ParentId == user.DeptId).Select(d => d.Id).ToListAsync();
                var ids = new List<long> { user.DeptId ?? 0 };
                ids.AddRange(childDepts);
                return ids.ToArray();
            case 4: default: return Array.Empty<long>(); // 仅本人
        }
    }
}
