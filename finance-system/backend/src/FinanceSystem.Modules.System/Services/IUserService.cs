using FinanceSystem.Core.Common;
using FinanceSystem.Modules.System.DTOs;
using FinanceSystem.Modules.System.Entities;

namespace FinanceSystem.Modules.System.Services;

/// <summary>
/// 用户管理服务接口
/// </summary>
/// <summary>
/// 用户管理服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>用户分页数据</returns>
    Task<PageResult<SysUser>> GetPageAsync(UserQuery query);

    /// <summary>
    /// 根据ID查询用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户详情（含角色）</returns>
    Task<SysUser?> GetByIdAsync(long id);

    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="request">新增请求</param>
    /// <returns>新用户ID</returns>
    Task<long> CreateAsync(UserCreateRequest request);

    /// <summary>
    /// 编辑用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">编辑请求</param>
    Task UpdateAsync(long id, UserCreateRequest request);

    /// <summary>
    /// 删除用户（校验关联）
    /// </summary>
    /// <param name="id">用户ID</param>
    Task DeleteAsync(long id);

    /// <summary>
    /// 切换用户状态
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="status">目标状态</param>
    Task ToggleStatusAsync(long id, int status);

    /// <summary>
    /// 重置用户密码为默认密码
    /// </summary>
    /// <param name="id">用户ID</param>
    Task ResetPasswordAsync(long id);

    /// <summary>
    /// 获取个人信息
    /// </summary>
    /// <param name="userId">当前用户ID</param>
    /// <returns>用户详情</returns>
    Task<UserProfile?> GetProfileAsync(long userId);

    /// <summary>
    /// 修改个人信息
    /// </summary>
    /// <param name="userId">当前用户ID</param>
    /// <param name="request">修改请求</param>
    Task UpdateProfileAsync(long userId, ProfileUpdateRequest request);
}
