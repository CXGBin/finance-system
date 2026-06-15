using FinanceSystem.Core.Common;

namespace FinanceSystem.Modules.System.DTOs;

/// <summary>
/// 字典类型查询条件
/// </summary>
/// <summary>
/// DictTypeQuery
/// </summary>
public class DictTypeQuery : PageRequest
{
    /// <summary>
    /// 字典名称模糊搜索
    /// </summary>
    public string? DictName { get; set; }

    /// <summary>
    /// 字典编码搜索
    /// </summary>
    public string? DictType { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 新增/编辑字典类型请求
/// </summary>
/// <summary>
/// DictTypeCreateRequest
/// </summary>
public class DictTypeCreateRequest
{
    /// <summary>
    /// 字典名称
    /// </summary>
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型编码
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 新增/编辑字典项请求
/// </summary>
/// <summary>
/// DictDataCreateRequest
/// </summary>
public class DictDataCreateRequest
{
    /// <summary>
    /// 所属字典类型编码
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典标签（显示值）
    /// </summary>
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典值（实际值）
    /// </summary>
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 操作日志查询条件
/// </summary>
/// <summary>
/// LogQuery
/// </summary>
public class LogQuery : PageRequest
{
    /// <summary>
    /// 所属模块筛选
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 操作类型筛选
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 操作人姓名模糊搜索
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 批量修改系统配置请求
/// </summary>
/// <summary>
/// ConfigUpdateRequest
/// </summary>
public class ConfigUpdateRequest
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 配置值
    /// </summary>
    public string ConfigValue { get; set; } = string.Empty;
}

/// <summary>
/// 模块开关切换请求
/// </summary>
/// <summary>
/// ModuleToggleRequest
/// </summary>
public class ModuleToggleRequest
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}
