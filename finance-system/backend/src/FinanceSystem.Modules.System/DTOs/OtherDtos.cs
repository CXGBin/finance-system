namespace FinanceSystem.Modules.System.DTOs;

/// <summary>
/// 新增/编辑菜单请求
/// </summary>
public class MenuCreateRequest
{
    /// <summary>
    /// 父级菜单ID（0为顶级）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1目录 2菜单 3按钮）
    /// </summary>
    public int MenuType { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 前端组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 所属模块标识
    /// </summary>
    public string? ModuleId { get; set; }

    /// <summary>
    /// 排序号
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
}

/// <summary>
/// 新增/编辑部门请求
/// </summary>
public class DeptCreateRequest
{
    /// <summary>
    /// 父级部门ID（0为顶级）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 状态（0停用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;
}

/// <summary>
/// 新增/编辑岗位请求
/// </summary>
public class PostCreateRequest
{
    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 岗位编码
    /// </summary>
    public string PostCode { get; set; } = string.Empty;

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
