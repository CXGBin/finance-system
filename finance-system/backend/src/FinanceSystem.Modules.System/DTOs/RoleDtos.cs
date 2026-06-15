using FinanceSystem.Core.Common;

namespace FinanceSystem.Modules.System.DTOs;

/// <summary>
/// 角色查询条件
/// </summary>
public class RoleQuery : PageRequest
{
    /// <summary>
    /// 角色名称模糊搜索
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色编码搜索
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 新增/编辑角色请求
/// </summary>
public class RoleCreateRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（大写字母+下划线）
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 菜单权限ID列表
    /// </summary>
    public List<long> MenuIds { get; set; } = new();
}

/// <summary>
/// 公告创建/修改请求
/// </summary>
public class NoticeCreateRequest
{
    /// <summary>公告标题</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>公告内容</summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>公告类型（1通知 2公告）</summary>
    public int NoticeType { get; set; } = 1;
    /// <summary>状态（0禁用 1启用）</summary>
    public int Status { get; set; } = 1;
}
