using FinanceSystem.Core.Interfaces;
using SqlSugar;

namespace FinanceSystem.Infrastructure.Services;

/// <summary>
/// SqlSugar 泛型仓储实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <summary>
/// Repository
/// </summary>
public class Repository<T> : IRepository<T> where T : class, new()
{
    /// <summary>
    /// SqlSugar 客户端实例
    /// </summary>
    protected readonly ISqlSugarClient Db;

    /// <summary>
    /// 创建仓储实例
    /// </summary>
    /// <param name="db">SqlSugar 客户端</param>
    public Repository(ISqlSugarClient db)
    {
        Db = db;
    }

    /// <summary>
    /// 根据主键查询单条记录
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>实体实例，未找到返回null</returns>
    public async Task<T?> GetByIdAsync(long id)
    {
        return await Db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 查询全部记录
    /// </summary>
    /// <returns>实体列表</returns>
    public async Task<List<T>> GetAllAsync()
    {
        return await Db.Queryable<T>().ToListAsync();
    }

    /// <summary>
    /// 新增记录
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <returns>插入后主键</returns>
    public async Task<long> InsertAsync(T entity)
    {
        return await Db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 批量新增记录
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>插入条数</returns>
    public async Task<int> InsertRangeAsync(List<T> entities)
    {
        return await Db.Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <returns>影响行数</returns>
    public async Task<int> UpdateAsync(T entity)
    {
        return await Db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据主键删除记录
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>影响行数</returns>
    public async Task<int> DeleteAsync(long id)
    {
        return await Db.Deleteable<T>().In(id).ExecuteCommandAsync();
    }
}
