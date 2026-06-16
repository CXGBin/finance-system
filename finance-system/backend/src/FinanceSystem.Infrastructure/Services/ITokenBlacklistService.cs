namespace FinanceSystem.Infrastructure.Services;

/// <summary>
/// Token黑名单服务接口（支持Redis和内存降级）
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// 将Token加入黑名单
    /// </summary>
    /// <param name="token">要加入黑名单的Token</param>
    /// <param name="expireMinutes">过期时间（分钟）</param>
    Task AddAsync(string token, int expireMinutes);

    /// <summary>
    /// 检查Token是否在黑名单中
    /// </summary>
    /// <param name="token">要检查的Token</param>
    /// <returns>是否在黑名单中</returns>
    Task<bool> IsBlacklistedAsync(string token);
}

/// <summary>
/// RefreshToken存储服务接口（支持Redis和内存降级）
/// </summary>
public interface IRefreshTokenStoreService
{
    /// <summary>
    /// 存储RefreshToken
    /// </summary>
    /// <param name="token">RefreshToken值</param>
    /// <param name="userId">关联用户ID</param>
    /// <param name="expireDays">过期天数</param>
    Task StoreAsync(string token, long userId, int expireDays);

    /// <summary>
    /// 获取RefreshToken关联的用户ID
    /// </summary>
    /// <param name="token">RefreshToken值</param>
    /// <returns>关联的用户ID，不存在返回null</returns>
    Task<long?> GetUserIdAsync(string token);

    /// <summary>
    /// 移除RefreshToken
    /// </summary>
    /// <param name="token">RefreshToken值</param>
    Task RemoveAsync(string token);
}
