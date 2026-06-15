using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 菜单管理服务接口
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// 获取完整菜单树
    /// </summary>
    /// <returns>菜单树形列表</returns>
    Task<List<SysMenu>> GetTreeAsync();

    /// <summary>
    /// 新增菜单
    /// </summary>
    /// <param name="request">新增请求</param>
    /// <returns>新菜单ID</returns>
    Task<long> CreateAsync(MenuCreateRequest request);

    /// <summary>
    /// 编辑菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateAsync(long id, MenuCreateRequest request);

    /// <summary>
    /// 删除菜单（级联删除子菜单）
    /// </summary>
    /// <param name="id">菜单ID</param>
    Task DeleteAsync(long id);
}

/// <summary>
/// 部门管理服务接口
/// </summary>
public interface IDeptService
{
    /// <summary>
    /// 获取部门树
    /// </summary>
    /// <returns>部门树形列表</returns>
    Task<List<SysDept>> GetTreeAsync();

    /// <summary>
    /// 新增部门
    /// </summary>
    /// <param name="request">新增请求</param>
    /// <returns>新部门ID</returns>
    Task<long> CreateAsync(DeptCreateRequest request);

    /// <summary>
    /// 编辑部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateAsync(long id, DeptCreateRequest request);

    /// <summary>
    /// 删除部门（校验关联用户）
    /// </summary>
    /// <param name="id">部门ID</param>
    Task DeleteAsync(long id);
}

/// <summary>
/// 岗位管理服务接口
/// </summary>
public interface IPostService
{
    /// <summary>
    /// 分页查询岗位列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <param name="deptId">部门ID筛选（可选）</param>
    /// <param name="postName">岗位名称模糊搜索（可选）</param>
    /// <returns>岗位分页数据</returns>
    Task<Core.Common.PageResult<SysPost>> GetPageAsync(int pageIndex, int pageSize, long? deptId = null, string? postName = null);

    /// <summary>
    /// 新增岗位
    /// </summary>
    /// <param name="request">新增请求</param>
    /// <returns>新岗位ID</returns>
    Task<long> CreateAsync(PostCreateRequest request);

    /// <summary>
    /// 编辑岗位
    /// </summary>
    /// <param name="id">岗位ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateAsync(long id, PostCreateRequest request);

    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="id">岗位ID</param>
    Task DeleteAsync(long id);
}

/// <summary>
/// 数据字典服务接口
/// </summary>
public interface IDictService
{
    /// <summary>
    /// 分页查询字典类型列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>字典类型分页数据</returns>
    Task<Core.Common.PageResult<SysDictType>> GetTypePageAsync(DictTypeQuery query);

    /// <summary>
    /// 根据字典类型编码获取所有字典项
    /// </summary>
    /// <param name="dictType">字典类型编码</param>
    /// <returns>字典项列表</returns>
    Task<List<SysDictData>> GetDataByTypeAsync(string dictType);

    /// <summary>
    /// 新增字典类型
    /// </summary>
    /// <param name="request">新增请求</param>
    Task CreateTypeAsync(DictTypeCreateRequest request);

    /// <summary>
    /// 编辑字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateTypeAsync(long id, DictTypeCreateRequest request);

    /// <summary>
    /// 删除字典类型（含关联字典项）
    /// </summary>
    /// <param name="id">字典类型ID</param>
    Task DeleteTypeAsync(long id);

    /// <summary>
    /// 新增字典项
    /// </summary>
    /// <param name="request">新增请求</param>
    Task CreateDataAsync(DictDataCreateRequest request);

    /// <summary>
    /// 编辑字典项
    /// </summary>
    /// <param name="id">字典项ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateDataAsync(long id, DictDataCreateRequest request);

    /// <summary>
    /// 删除字典项
    /// </summary>
    /// <param name="id">字典项ID</param>
    Task DeleteDataAsync(long id);
}

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 分页查询操作日志
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>日志分页数据</returns>
    Task<Core.Common.PageResult<SysLog>> GetPageAsync(LogQuery query);

    /// <summary>
    /// 根据ID查看日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>日志详情</returns>
    Task<SysLog?> GetByIdAsync(long id);
}

/// <summary>
/// 模块管理服务接口
/// </summary>
public interface IModuleService
{
    /// <summary>
    /// 获取所有模块列表
    /// </summary>
    /// <returns>模块列表</returns>
    Task<List<SysModule>> GetListAsync();

    /// <summary>
    /// 切换模块开关状态
    /// </summary>
    /// <param name="moduleId">模块标识</param>
    /// <param name="isEnabled">是否启用</param>
    Task ToggleAsync(string moduleId, bool isEnabled);

    /// <summary>
    /// 获取模块依赖关系
    /// </summary>
    /// <param name="moduleId">模块标识</param>
    /// <returns>依赖模块ID列表</returns>
    Task<List<string>> GetDependenciesAsync(string moduleId);
}

/// <summary>
/// 系统配置服务接口
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// 获取所有配置项
    /// </summary>
    /// <returns>配置列表</returns>
    Task<List<SysConfig>> GetListAsync();

    /// <summary>
    /// 批量修改配置
    /// </summary>
    /// <param name="items">配置修改列表</param>
    Task BatchUpdateAsync(List<ConfigUpdateRequest> items);
}
