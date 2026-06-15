using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.System.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Modules.System.Controllers;

/// <summary>
/// 数据字典管理控制器
/// </summary>
[ApiController]
[Route("api/system/dict")]
public class DictController : ControllerBase
{
    private readonly IDictService _dictService;

    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    /// <summary>
    /// 分页查询字典类型
    /// </summary>
    [HttpGet("type/page")]
    public async Task<ApiResult<PageResult<SysDictType>>> GetTypePage([FromQuery] DictTypeQuery query)
    {
        return ApiResult<PageResult<SysDictType>>.Success(await _dictService.GetTypePageAsync(query));
    }

    /// <summary>
    /// 新增字典类型
    /// </summary>
    [HttpPost("type")]
    public async Task<ApiResult<bool>> CreateType([FromBody] DictTypeCreateRequest request)
    {
        await _dictService.CreateTypeAsync(request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 编辑字典类型
    /// </summary>
    [HttpPut("type/{id}")]
    public async Task<ApiResult<bool>> UpdateType(long id, [FromBody] DictTypeCreateRequest request)
    {
        await _dictService.UpdateTypeAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除字典类型（含关联字典项）
    /// </summary>
    [HttpDelete("type/{id}")]
    public async Task<ApiResult<bool>> DeleteType(long id)
    {
        await _dictService.DeleteTypeAsync(id);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 获取某类型下所有字典项
    /// </summary>
    [HttpGet("data/{dictType}")]
    public async Task<ApiResult<List<SysDictData>>> GetDataByType(string dictType)
    {
        return ApiResult<List<SysDictData>>.Success(await _dictService.GetDataByTypeAsync(dictType));
    }

    /// <summary>
    /// 新增字典项
    /// </summary>
    [HttpPost("data")]
    public async Task<ApiResult<bool>> CreateData([FromBody] DictDataCreateRequest request)
    {
        await _dictService.CreateDataAsync(request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 编辑字典项
    /// </summary>
    [HttpPut("data/{id}")]
    public async Task<ApiResult<bool>> UpdateData(long id, [FromBody] DictDataCreateRequest request)
    {
        await _dictService.UpdateDataAsync(id, request);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 删除字典项
    /// </summary>
    [HttpDelete("data/{id}")]
    public async Task<ApiResult<bool>> DeleteData(long id)
    {
        await _dictService.DeleteDataAsync(id);
        return ApiResult<bool>.Success(true);
    }
}

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiController]
[Route("api/system/log")]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;

    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 分页查询操作日志
    /// </summary>
    [HttpGet("page")]
    public async Task<ApiResult<PageResult<SysLog>>> GetPage([FromQuery] LogQuery query)
    {
        return ApiResult<PageResult<SysLog>>.Success(await _logService.GetPageAsync(query));
    }

    /// <summary>
    /// 查看日志详情（含请求体）
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResult<SysLog>> GetById(long id)
    {
        var log = await _logService.GetByIdAsync(id);
        return log == null
            ? ApiResult<SysLog>.Fail("日志不存在")
            : ApiResult<SysLog>.Success(log);
    }
}

/// <summary>
/// 模块管理控制器
/// </summary>
[ApiController]
[Route("api/system/module")]
public class ModuleController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModuleController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    /// <summary>
    /// 获取模块列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<SysModule>>> GetList()
    {
        return ApiResult<List<SysModule>>.Success(await _moduleService.GetListAsync());
    }

    /// <summary>
    /// 切换模块开关
    /// </summary>
    [HttpPut("{moduleId}/toggle")]
    public async Task<ApiResult<bool>> Toggle(string moduleId, [FromBody] ModuleToggleRequest request)
    {
        await _moduleService.ToggleAsync(moduleId, request.IsEnabled);
        return ApiResult<bool>.Success(true);
    }

    /// <summary>
    /// 获取模块依赖关系
    /// </summary>
    [HttpGet("{moduleId}/dependencies")]
    public async Task<ApiResult<List<string>>> GetDependencies(string moduleId)
    {
        return ApiResult<List<string>>.Success(await _moduleService.GetDependenciesAsync(moduleId));
    }
}

/// <summary>
/// 系统配置控制器
/// </summary>
[ApiController]
[Route("api/system/config")]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _configService;

    public ConfigController(IConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>
    /// 获取所有配置项
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<List<SysConfig>>> GetList()
    {
        return ApiResult<List<SysConfig>>.Success(await _configService.GetListAsync());
    }

    /// <summary>
    /// 批量修改配置
    /// </summary>
    [HttpPut]
    public async Task<ApiResult<bool>> BatchUpdate([FromBody] List<ConfigUpdateRequest> items)
    {
        await _configService.BatchUpdateAsync(items);
        return ApiResult<bool>.Success(true);
    }
}
