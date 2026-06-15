using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统用户实体
/// </summary>
[SugarTable("sys_user", "系统用户表")]
/// <summary>
/// SysUser
/// </summary>
public class SysUser : FullEntity
{
    /// <summary>
    /// 用户名（唯一）
    /// </summary>
    [SugarColumn(Length = 50)]
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希（BCrypt）
    /// </summary>
    [SugarColumn(Length = 256)]
    public string? PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
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
    /// 状态（0禁用 1启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 登录失败次数（用于连续错误锁定）
    /// </summary>
    public int LoginFailCount { get; set; }

    /// <summary>
    /// 账户锁定截止时间
    /// </summary>
    public DateTime? LockoutEndTime { get; set; }

    /// <summary>
    /// 用户角色列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRole>? Roles { get; set; }

    /// <summary>
    /// 用户菜单权限列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysMenu>? Menus { get; set; }
}
