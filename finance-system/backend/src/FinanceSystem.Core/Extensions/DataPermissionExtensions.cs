using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace FinanceSystem.Core.Extensions;

/// <summary>
/// 数据权限扩展方法，为ISqlSugarClient查询添加部门级数据过滤
/// </summary>
public static class DataPermissionExtensions
{
    /// <summary>
    /// 应用数据权限过滤（根据用户角色和数据范围配置）
    /// 超级管理员不过滤，普通用户按部门过滤
    /// </summary>
    /// <param name="query">SqlSugar查询对象</param>
    /// <param name="context">HttpContext（获取当前用户信息）</param>
    /// <param name="deptIdColumn">部门ID列名（默认"DeptId"）</param>
    /// <param name="userIdColumn">用户ID列名（默认"CreatedBy"或"PreparedBy"）</param>
    /// <returns>应用过滤后的查询对象</returns>
    public static ISugarQueryable<T> ApplyDataPermission<T>(
        this ISugarQueryable<T> query,
        HttpContext context,
        string deptIdColumn = "DeptId",
        string? userIdColumn = null)
        where T : class, new()
    {
        var userId = context.GetCurrentUserId();
        if (userId <= 0) return query;

        // 检查是否是超级管理员（通过JWT角色Claim判断）
        var roles = context.User?.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value).ToList();

        // 超级管理员拥有全部数据权限，不过滤
        if (roles != null && roles.Any(r => r == "SUPER_ADMIN" || r == "admin"))
        {
            return query;
        }

        // 获取用户所属部门ID（从Claim中读取）
        var deptIdStr = context.User?.FindFirst("DeptId")?.Value;
        if (long.TryParse(deptIdStr, out var deptId) && deptId > 0)
        {
            // 按部门过滤：本部门+子部门数据
            query = query.Where($"{deptIdColumn} = @deptId", new { deptId });
        }
        else if (!string.IsNullOrEmpty(userIdColumn))
        {
            // 无部门信息时按用户ID过滤
            query = query.Where($"{userIdColumn} = @userId", new { userId });
        }

        return query;
    }
}
