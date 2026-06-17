using SqlSugar;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceSystem.Core.Extensions;

/// <summary>
/// SqlSugar 查询排序扩展
/// </summary>
public static class QueryableSortExtensions
{
    /// <summary>
    /// 根据排序字段和方向动态排序
    /// 支持 camelCase 和 PascalCase 字段名
    /// </summary>
    public static ISugarQueryable<T> ApplySort<T>(this ISugarQueryable<T> queryable, string? sortField, string? sortOrder)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(sortField))
            return queryable;

        var orderByType = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase)
            ? OrderByType.Asc
            : OrderByType.Desc;

        // 遍历实体属性查找匹配的字段（支持 camelCase/PascalCase）
        var prop = typeof(T).GetProperty(sortField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop == null)
            return queryable;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, prop);
        var conversion = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

        return queryable.OrderBy(lambda, orderByType);
    }
}
