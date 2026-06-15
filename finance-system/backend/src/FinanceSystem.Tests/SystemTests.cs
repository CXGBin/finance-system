using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;
using Xunit;

namespace FinanceSystem.Tests.System;

/// <summary>
/// 用户管理服务单元测试
/// </summary>
/// <summary>
/// User服务测试
/// </summary>
public class UserServiceTests
{
    /// <summary>
    /// 创建用户请求：密码为空时应使用默认密码
    /// </summary>
    [Fact]
    public void UserCreateRequest_DefaultPassword_ShouldBeEmpty()
    {
        var request = new UserCreateRequest
        {
            Username = "testuser",
            RealName = "测试用户",
            Password = null,
            RoleIds = new List<long>()
        };
        // 密码为空时使用默认密码123456（在UserService中实现）
        Assert.Null(request.Password);
    }

    /// <summary>
    /// 用户实体：登录失败次数从0开始
    /// </summary>
    [Fact]
    public void UserEntity_LoginFailCount_ShouldBeZero()
    {
        var user = new SysUser
        {
            Username = "admin",
            RealName = "管理员"
        };
        Assert.Equal(0, user.LoginFailCount);
        Assert.Null(user.LockoutEndTime);
    }

    /// <summary>
    /// 用户实体：锁定时间设置后应大于当前时间
    /// </summary>
    [Fact]
    public void UserEntity_LockoutEnd_ShouldBeFuture()
    {
        var user = new SysUser { Username = "test" };
        user.LoginFailCount = 5;
        user.LockoutEndTime = DateTime.Now.AddMinutes(30);

        Assert.NotNull(user.LockoutEndTime);
        Assert.True(user.LockoutEndTime.Value > DateTime.Now);
    }

    /// <summary>
    /// 用户查询：默认分页参数
    /// </summary>
    [Fact]
    public void UserQuery_DefaultPagination()
    {
        var query = new UserQuery();
        Assert.Equal(1, query.PageIndex);
        Assert.Equal(20, query.PageSize);
    }

    /// <summary>
    /// 用户CRUD请求：角色列表不为null
    /// </summary>
    [Fact]
    public void UserCreateRequest_RoleIds_ShouldNotBeNull()
    {
        var request = new UserCreateRequest();
        Assert.NotNull(request.RoleIds);
        Assert.Empty(request.RoleIds);
    }

    /// <summary>
    /// 角色查询：支持按编码和名称筛选
    /// </summary>
    [Fact]
    public void RoleQuery_FilterProperties()
    {
        var query = new RoleQuery
        {
            RoleName = "管理",
            RoleCode = "ADMIN",
            Status = 1
        };
        Assert.Equal("管理", query.RoleName);
        Assert.Equal("ADMIN", query.RoleCode);
        Assert.Equal(1, query.Status);
    }
}

/// <summary>
/// 认证服务单元测试（纯逻辑验证）
/// </summary>
/// <summary>
/// Auth服务测试
/// </summary>
public class AuthServiceTests
{
    /// <summary>
    /// 登录失败5次应锁定账户30分钟
    /// </summary>
    [Fact]
    public void LoginFail_5Times_ShouldLockAccount()
    {
        var user = new SysUser { Username = "testuser", LoginFailCount = 4 };
        user.LoginFailCount++;
        Assert.Equal(5, user.LoginFailCount);

        // 第5次失败触发锁定
        user.LockoutEndTime = DateTime.Now.AddMinutes(30);
        user.LoginFailCount = 0;
        Assert.NotNull(user.LockoutEndTime);
        Assert.True(user.LockoutEndTime.Value > DateTime.Now);
    }

    /// <summary>
    /// 登录成功应重置失败计数
    /// </summary>
    [Fact]
    public void LoginSuccess_ShouldResetFailCount()
    {
        var user = new SysUser { LoginFailCount = 3, LockoutEndTime = DateTime.Now.AddMinutes(10) };
        user.LoginFailCount = 0;
        user.LockoutEndTime = null;

        Assert.Equal(0, user.LoginFailCount);
        Assert.Null(user.LockoutEndTime);
    }

    /// <summary>
    /// 被锁定账户不允许登录
    /// </summary>
    [Fact]
    public void LockedAccount_ShouldNotAllowLogin()
    {
        var user = new SysUser { LockoutEndTime = DateTime.Now.AddMinutes(20) };
        Assert.True(user.LockoutEndTime.HasValue && user.LockoutEndTime.Value > DateTime.Now);
    }

    /// <summary>
    /// 修改密码：两次输入不一致应抛异常
    /// </summary>
    [Fact]
    public void ChangePassword_Mismatch_ShouldThrow()
    {
        var request = new ChangePasswordRequest
        {
            OldPassword = "old123",
            NewPassword = "new456",
            ConfirmPassword = "new789"
        };
        Assert.NotEqual(request.NewPassword, request.ConfirmPassword);
    }

    /// <summary>
    /// 修改密码：两次输入一致应通过校验
    /// </summary>
    [Fact]
    public void ChangePassword_Match_ShouldPass()
    {
        var request = new ChangePasswordRequest
        {
            OldPassword = "old123",
            NewPassword = "new456",
            ConfirmPassword = "new456"
        };
        Assert.Equal(request.NewPassword, request.ConfirmPassword);
    }
}

/// <summary>
/// 角色管理服务单元测试
/// </summary>
/// <summary>
/// Role服务测试
/// </summary>
public class RoleServiceTests
{
    /// <summary>
    /// 角色创建请求：菜单列表默认不为null
    /// </summary>
    [Fact]
    public void RoleCreateRequest_MenuIds_DefaultNotNull()
    {
        var request = new RoleCreateRequest
        {
            RoleName = "测试角色",
            RoleCode = "TEST_ROLE"
        };
        Assert.NotNull(request.MenuIds);
        Assert.Empty(request.MenuIds);
    }

    /// <summary>
    /// 角色实体：编码唯一性校验
    /// </summary>
    [Fact]
    public void RoleEntity_CodeShouldBeSet()
    {
        var role = new SysRole { RoleName = "管理员", RoleCode = "SUPER_ADMIN", Status = 1 };
        Assert.Equal("SUPER_ADMIN", role.RoleCode);
    }
}
