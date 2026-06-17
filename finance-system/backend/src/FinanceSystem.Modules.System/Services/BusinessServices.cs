using FinanceSystem.Core.Common;
using FinanceSystem.Core.Extensions;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 菜单管理服务实现
/// </summary>
/// <summary>
/// 菜单管理服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly ISqlSugarClient _db;

    public MenuService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SysMenu>> GetTreeAsync()
    {
        var allMenus = await _db.Queryable<SysMenu>()
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
        return BuildTree(allMenus, 0);
    }

    /// <inheritdoc/>
    public async Task<SysMenu?> GetByIdAsync(long id) => await _db.Queryable<SysMenu>().FirstAsync(m => m.Id == id);

    /// <inheritdoc/>
    public async Task<long> CreateAsync(MenuCreateRequest request)
    {
        var menu = new SysMenu
        {
            ParentId = request.ParentId,
            MenuName = request.MenuName,
            MenuType = request.MenuType,
            Path = request.Path,
            Component = request.Component,
            Permission = request.Permission,
            Icon = request.Icon,
            ModuleId = request.ModuleId,
            SortOrder = request.SortOrder,
            Visible = request.Visible,
            Status = request.Status
        };
        await _db.Insertable(menu).ExecuteCommandAsync();
        return menu.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, MenuCreateRequest request)
    {
        var menu = await _db.Queryable<SysMenu>().FirstAsync(m => m.Id == id)
            ?? throw new NotFoundException("菜单不存在");
        menu.MenuName = request.MenuName;
        menu.MenuType = request.MenuType;
        menu.Path = request.Path;
        menu.Component = request.Component;
        menu.Permission = request.Permission;
        menu.Icon = request.Icon;
        menu.ModuleId = request.ModuleId;
        menu.SortOrder = request.SortOrder;
        menu.Visible = request.Visible;
        menu.Status = request.Status;
        await _db.Updateable(menu).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        // 查询所有子菜单ID（递归）
        var allMenus = await _db.Queryable<SysMenu>().ToListAsync();
        var idsToDelete = GetChildIds(allMenus, id);
        idsToDelete.Add(id);
        // 删除关联的角色菜单
        await _db.Deleteable<SysRoleMenu>()
            .Where(rm => idsToDelete.Contains(rm.MenuId))
            .ExecuteCommandAsync();
        // 删除菜单（含子菜单）
        await _db.Deleteable<SysMenu>()
            .Where(m => idsToDelete.Contains(m.Id))
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 构建树形结构
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="parentId">父级ID</param>
    /// <returns>树形菜单列表</returns>
    private List<SysMenu> BuildTree(List<SysMenu> allMenus, long parentId)
    {
        return allMenus
            .Where(m => m.ParentId == parentId)
            .Select(m =>
            {
                m.Children = BuildTree(allMenus, m.Id);
                return m;
            })
            .ToList();
    }

    /// <summary>
    /// 递归获取所有子菜单ID
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="parentId">父级ID</param>
    /// <returns>子菜单ID列表</returns>
    private List<long> GetChildIds(List<SysMenu> allMenus, long parentId)
    {
        var ids = new List<long>();
        var children = allMenus.Where(m => m.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            ids.Add(child.Id);
            ids.AddRange(GetChildIds(allMenus, child.Id));
        }
        return ids;
    }
}

/// <summary>
/// 部门管理服务实现
/// </summary>
/// <summary>
/// 部门管理服务实现
/// </summary>
public class DeptService : IDeptService
{
    private readonly ISqlSugarClient _db;

    public DeptService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SysDept>> GetTreeAsync()
    {
        var allDepts = await _db.Queryable<SysDept>()
            .OrderBy(d => d.SortOrder)
            .ToListAsync();
        return BuildTree(allDepts, 0);
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(DeptCreateRequest request)
    {
        var dept = new SysDept
        {
            ParentId = request.ParentId,
            DeptName = request.DeptName,
            Leader = request.Leader,
            Phone = request.Phone,
            Email = request.Email,
            SortOrder = request.SortOrder,
            Status = request.Status
        };
        await _db.Insertable(dept).ExecuteCommandAsync();
        return dept.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, DeptCreateRequest request)
    {
        var dept = await _db.Queryable<SysDept>().FirstAsync(d => d.Id == id)
            ?? throw new NotFoundException("部门不存在");
        dept.DeptName = request.DeptName;
        dept.SortOrder = request.SortOrder;
        dept.Leader = request.Leader;
        dept.Phone = request.Phone;
        dept.Email = request.Email;
        dept.Status = request.Status;
        await _db.Updateable(dept).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        // 校验是否有关联用户
        var hasUser = await _db.Queryable<SysUser>().AnyAsync(u => u.DeptId == id);
        if (hasUser)
            throw new BusinessException("该部门下有关联用户，请先转移用户后再删除");
        // 递归删除子部门
        var allDepts = await _db.Queryable<SysDept>().ToListAsync();
        var idsToDelete = GetChildIds(allDepts, id);
        idsToDelete.Add(id);
        await _db.Deleteable<SysDept>()
            .Where(d => idsToDelete.Contains(d.Id))
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 构建部门树形结构
    /// </summary>
    /// <param name="allDepts">所有部门列表</param>
    /// <param name="parentId">父级ID</param>
    /// <returns>树形部门列表</returns>
    private List<SysDept> BuildTree(List<SysDept> allDepts, long parentId)
    {
        return allDepts
            .Where(d => d.ParentId == parentId)
            .Select(d =>
            {
                d.Children = BuildTree(allDepts, d.Id);
                return d;
            })
            .ToList();
    }

    /// <summary>
    /// 递归获取所有子部门ID
    /// </summary>
    /// <param name="allDepts">所有部门列表</param>
    /// <param name="parentId">父级ID</param>
    /// <returns>子部门ID列表</returns>
    private List<long> GetChildIds(List<SysDept> allDepts, long parentId)
    {
        var ids = new List<long>();
        var children = allDepts.Where(d => d.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            ids.Add(child.Id);
            ids.AddRange(GetChildIds(allDepts, child.Id));
        }
        return ids;
    }
}

/// <summary>
/// 岗位管理服务实现
/// </summary>
/// <summary>
/// 岗位管理服务实现
/// </summary>
public class PostService : IPostService
{
    private readonly ISqlSugarClient _db;

    public PostService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<SysPost>> GetPageAsync(int pageIndex, int pageSize, long? deptId = null, string? postName = null)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<SysPost>()
            .WhereIF(deptId.HasValue, p => p.DeptId == deptId)
            .WhereIF(!string.IsNullOrEmpty(postName), p => p.PostName.Contains(postName!))
            .OrderBy(p => p.SortOrder)
            .ToPageListAsync(pageIndex, pageSize, total);
        return new PageResult<SysPost>(total, list);
    }

    /// <inheritdoc/>
    public async Task<long> CreateAsync(PostCreateRequest request)
    {
        var exists = await _db.Queryable<SysPost>().AnyAsync(p => p.PostCode == request.PostCode);
        if (exists)
            throw new BusinessException($"岗位编码 '{request.PostCode}' 已存在");
        var post = new SysPost
        {
            DeptId = request.DeptId,
            PostName = request.PostName,
            PostCode = request.PostCode,
            SortOrder = request.SortOrder,
            Status = request.Status,
            Remark = request.Remark
        };
        await _db.Insertable(post).ExecuteCommandAsync();
        return post.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(long id, PostCreateRequest request)
    {
        var post = await _db.Queryable<SysPost>().FirstAsync(p => p.Id == id)
            ?? throw new NotFoundException("岗位不存在");
        post.DeptId = request.DeptId;
        post.PostName = request.PostName;
        post.PostCode = request.PostCode;
        post.SortOrder = request.SortOrder;
        post.Status = request.Status;
        post.Remark = request.Remark;
        post.UpdatedTime = DateTime.Now;
        await _db.Updateable(post).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id)
    {
        // 校验是否有关联用户
        var hasUser = await _db.Queryable<SysUser>().AnyAsync(u => u.PostId == id);
        if (hasUser)
            throw new BusinessException("该岗位已分配给用户，请先取消关联后再删除");
        await _db.Deleteable<SysPost>().Where(p => p.Id == id).ExecuteCommandAsync();
    }
}

/// <summary>
/// 数据字典服务实现
/// </summary>
/// <summary>
/// 字典管理服务实现
/// </summary>
public class DictService : IDictService
{
    private readonly ISqlSugarClient _db;

    public DictService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<SysDictType>> GetTypePageAsync(DictTypeQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<SysDictType>()
            .WhereIF(!string.IsNullOrEmpty(query.DictName), d => d.DictName.Contains(query.DictName!))
            .WhereIF(!string.IsNullOrEmpty(query.DictType), d => d.DictType.Contains(query.DictType!))
            .WhereIF(query.Status.HasValue, d => d.Status == query.Status)
            .OrderBy(d => d.CreatedTime, OrderByType.Desc)
            .ApplySort(query.SortField, query.SortOrder).ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<SysDictType>(total, list);
    }

    /// <inheritdoc/>
    public async Task<List<SysDictData>> GetDataByTypeAsync(string dictType)
    {
        return await _db.Queryable<SysDictData>()
            .Where(d => d.DictType == dictType && d.Status == 1)
            .OrderBy(d => d.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task CreateTypeAsync(DictTypeCreateRequest request)
    {
        var exists = await _db.Queryable<SysDictType>().AnyAsync(d => d.DictType == request.DictType);
        if (exists)
            throw new BusinessException($"字典类型编码 '{request.DictType}' 已存在");
        await _db.Insertable(new SysDictType
        {
            DictName = request.DictName,
            DictType = request.DictType,
            Status = request.Status,
            Remark = request.Remark
        }).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateTypeAsync(long id, DictTypeCreateRequest request)
    {
        var dictType = await _db.Queryable<SysDictType>().FirstAsync(d => d.Id == id)
            ?? throw new NotFoundException("字典类型不存在");
        dictType.DictName = request.DictName;
        dictType.Status = request.Status;
        dictType.Remark = request.Remark;
        await _db.Updateable(dictType).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteTypeAsync(long id)
    {
        var dictType = await _db.Queryable<SysDictType>().FirstAsync(d => d.Id == id)
            ?? throw new NotFoundException("字典类型不存在");
        // 删除关联的字典项
        await _db.Deleteable<SysDictData>().Where(d => d.DictType == dictType.DictType).ExecuteCommandAsync();
        await _db.Deleteable<SysDictType>().Where(d => d.Id == id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task CreateDataAsync(DictDataCreateRequest request)
    {
        await _db.Insertable(new SysDictData
        {
            DictType = request.DictType,
            DictLabel = request.DictLabel,
            DictValue = request.DictValue,
            SortOrder = request.SortOrder,
            Status = request.Status,
            Remark = request.Remark
        }).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateDataAsync(long id, DictDataCreateRequest request)
    {
        var data = await _db.Queryable<SysDictData>().FirstAsync(d => d.Id == id)
            ?? throw new NotFoundException("字典项不存在");
        data.DictLabel = request.DictLabel;
        data.DictValue = request.DictValue;
        data.SortOrder = request.SortOrder;
        data.Status = request.Status;
        data.Remark = request.Remark;
        await _db.Updateable(data).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteDataAsync(long id)
    {
        await _db.Deleteable<SysDictData>().Where(d => d.Id == id).ExecuteCommandAsync();
    }
}

/// <summary>
/// 操作日志服务实现
/// </summary>
/// <summary>
/// 操作日志服务实现
/// </summary>
public class LogService : ILogService
{
    private readonly ISqlSugarClient _db;

    public LogService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<PageResult<SysLog>> GetPageAsync(LogQuery query)
    {
        RefAsync<int> total = 0;
        var list = await _db.Queryable<SysLog>()
            .WhereIF(!string.IsNullOrEmpty(query.Module), l => l.Module == query.Module)
            .WhereIF(!string.IsNullOrEmpty(query.Action), l => l.Action == query.Action)
            .WhereIF(!string.IsNullOrEmpty(query.UserName), l => l.UserName != null && l.UserName.Contains(query.UserName!))
            .WhereIF(query.StartTime.HasValue, l => l.CreatedTime >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, l => l.CreatedTime <= query.EndTime)
            .OrderBy(l => l.CreatedTime, OrderByType.Desc)
            .ApplySort(query.SortField, query.SortOrder).ToPageListAsync(query.PageIndex, query.PageSize, total);
        return new PageResult<SysLog>(total, list);
    }

    /// <inheritdoc/>
    public async Task<SysLog?> GetByIdAsync(long id)
    {
        return await _db.Queryable<SysLog>().FirstAsync(l => l.Id == id);
    }
}

/// <summary>
/// 模块管理服务实现
/// </summary>
/// <summary>
/// 模块管理服务实现
/// </summary>
public class ModuleService : IModuleService
{
    private readonly ISqlSugarClient _db;

    public ModuleService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SysModule>> GetListAsync()
    {
        return await _db.Queryable<SysModule>()
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task ToggleAsync(string moduleId, bool isEnabled)
    {
        var module = await _db.Queryable<SysModule>().FirstAsync(m => m.ModuleId == moduleId)
            ?? throw new NotFoundException($"模块 '{moduleId}' 不存在");

        // 核心模块不可关闭
        if (module.IsCore == 1 && !isEnabled)
            throw new BusinessException("核心模块不可关闭");

        // 关闭时检查依赖：如果有其他已启用模块依赖此模块，则禁止
        if (!isEnabled)
        {
            var allModules = await _db.Queryable<SysModule>().ToListAsync();
            var dependentModules = allModules.Where(m =>
                m.IsEnabled == 1 &&
                !string.IsNullOrEmpty(m.Dependencies) &&
                m.Dependencies.Split(',').Contains(moduleId));
            if (dependentModules.Any())
                throw new BusinessException($"请先关闭依赖此模块的模块：{string.Join("、", dependentModules.Select(m => m.ModuleName))}");
        }

        module.IsEnabled = isEnabled ? 1 : 0;
        module.UpdatedTime = DateTime.Now;
        await _db.Updateable(module)
            .UpdateColumns(m => new { m.IsEnabled, m.UpdatedTime })
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetDependenciesAsync(string moduleId)
    {
        var module = await _db.Queryable<SysModule>().FirstAsync(m => m.ModuleId == moduleId)
            ?? throw new NotFoundException($"模块 '{moduleId}' 不存在");
        if (string.IsNullOrEmpty(module.Dependencies))
            return new List<string>();
        return module.Dependencies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}

/// <summary>
/// 系统配置服务实现
/// </summary>
/// <summary>
/// 系统配置服务实现
/// </summary>
public class ConfigService : IConfigService
{
    private readonly ISqlSugarClient _db;

    public ConfigService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    public async Task<List<SysConfig>> GetListAsync()
    {
        return await _db.Queryable<SysConfig>().ToListAsync();
    }

    /// <inheritdoc/>
    public async Task BatchUpdateAsync(List<ConfigUpdateRequest> items)
    {
        foreach (var item in items)
        {
            await _db.Updateable<SysConfig>()
                .SetColumns(c => c.ConfigValue == item.ConfigValue)
                .SetColumns(c => c.CreatedTime == DateTime.Now)
                .Where(c => c.ConfigKey == item.ConfigKey)
                .ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task UpdateByKeyAsync(string key, ConfigUpdateRequest request)
    {
        await _db.Updateable<SysConfig>()
            .SetColumns(c => c.ConfigValue == request.ConfigValue)
            .SetColumns(c => c.CreatedTime == DateTime.Now)
            .Where(c => c.ConfigKey == key)
            .ExecuteCommandAsync();
    }
}

/// <summary>
/// 系统公告服务实现
/// </summary>
/// <summary>
/// 系统公告服务实现
/// </summary>
public class NoticeService : INoticeService
{
    private readonly ISqlSugarClient _db;
    public NoticeService(ISqlSugarClient db) => _db = db;

    public async Task<List<SysNotice>> GetListAsync(int? noticeType = null)
    {
        return await _db.Queryable<SysNotice>()
            .WhereIF(noticeType.HasValue, n => n.NoticeType == noticeType)
            .Where(n => n.Status == 1)
            .OrderByDescending(n => n.CreatedTime)
            .ToListAsync();
    }

    public async Task<long> CreateAsync(NoticeCreateRequest request, long currentUserId)
    {
        var entity = new SysNotice
        {
            Title = request.Title,
            Content = request.Content,
            NoticeType = request.NoticeType,
            Status = request.Status,
            CreatedBy = currentUserId
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(long id, NoticeCreateRequest request)
    {
        var entity = await _db.Queryable<SysNotice>().FirstAsync(n => n.Id == id)
            ?? throw new NotFoundException("公告不存在");
        entity.Title = request.Title;
        entity.Content = request.Content;
        entity.NoticeType = request.NoticeType;
        entity.Status = request.Status;
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _db.Deleteable<SysNotice>().Where(n => n.Id == id).ExecuteCommandAsync();
    }
}
