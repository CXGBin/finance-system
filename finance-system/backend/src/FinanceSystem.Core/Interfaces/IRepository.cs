namespace FinanceSystem.Core.Interfaces;

/// <summary>
/// 泛型仓储接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <summary>
/// IRepository
/// </summary>
public interface IRepository<T> where T : class, new()
{
    /// <summary>
    /// 根据主键查询单条记录
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>实体实例，未找到返回null</returns>
    Task<T?> GetByIdAsync(long id);

    /// <summary>
    /// 查询全部记录
    /// </summary>
    /// <returns>实体列表</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// 新增记录
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <returns>插入后主键</returns>
    Task<long> InsertAsync(T entity);

    /// <summary>
    /// 批量新增记录
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>插入条数</returns>
    Task<int> InsertRangeAsync(List<T> entities);

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <returns>影响行数</returns>
    Task<int> UpdateAsync(T entity);

    /// <summary>
    /// 根据主键删除记录
    /// </summary>
    /// <param name="id">主键值</param>
    /// <returns>影响行数</returns>
    Task<int> DeleteAsync(long id);
}
