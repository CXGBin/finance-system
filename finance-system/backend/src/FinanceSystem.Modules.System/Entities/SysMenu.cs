using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统菜单/权限实体
/// </summary>
[SugarTable("sys_menu", "系统菜单表")]
/// <summary>
/// SysMenu
/// </summary>
public class SysMenu : BaseEntity
{
    /// <summary>
    /// 父级菜单ID（0为顶级）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [SugarColumn(Length = 50)]
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1目录 2菜单 3按钮/权限）
    /// </summary>
    public int MenuType { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 前端组件路径
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识（按钮类型时必填，如 system:user:add）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Permission { get; set; }

    /// <summary>
    /// 菜单图标（antd图标名）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 所属模块标识
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ModuleId { get; set; }

    /// <summary>
    /// 排序号（同级内排序）
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可见（0隐藏 1显示）
    /// </summary>
    public int Visible { get; set; } = 1;

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 子菜单列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysMenu>? Children { get; set; }
}
