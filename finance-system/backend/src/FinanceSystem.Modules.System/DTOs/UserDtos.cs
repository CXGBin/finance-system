using FinanceSystem.Core.Common;

namespace FinanceSystem.Modules.System.DTOs;

/// <summary>
/// 用户查询条件
/// </summary>
/// <summary>
/// UserQuery
/// </summary>
public class UserQuery : PageRequest
{
    /// <summary>
    /// 用户名模糊搜索
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 真实姓名模糊搜索
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号搜索
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态筛选（0停用 1启用）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 部门ID筛选
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 角色ID筛选
    /// </summary>
    public long? RoleId { get; set; }

    /// <summary>
    /// 创建时间-开始
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 创建时间-结束
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 新增/编辑用户请求
/// </summary>
/// <summary>
/// UserCreateRequest
/// </summary>
public class UserCreateRequest
{
    /// <summary>
    /// 用户名（创建时必填，编辑时忽略）
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码（创建时必填，编辑时为空表示不修改）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 所属岗位ID
    /// </summary>
    public long? PostId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<long> RoleIds { get; set; } = new();

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
/// 用户个人信息（不含敏感字段）
/// </summary>
/// <summary>
/// UserProfile
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 所属部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 用户角色ID列表
    /// </summary>
    public List<long> RoleIds { get; set; } = new();

    /// <summary>
    /// 用户角色名称列表
    /// </summary>
    public List<string> RoleNames { get; set; } = new();
    /// <summary>
    /// 状态（0禁用 1启用）
    /// </summary>
    public int Status { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
}

/// <summary>
/// 修改个人信息请求
/// </summary>
/// <summary>
/// ProfileUpdateRequest
/// </summary>
public class ProfileUpdateRequest
{
    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }
}
