using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FinanceSystem.Infrastructure.Services;

/// <summary>
/// Redis Token黑名单服务实现
/// Redis不可用时自动降级到内存HashSet
/// </summary>
public class RedisTokenBlacklistService : ITokenBlacklistService, IRefreshTokenStoreService, IDisposable
{
    private readonly IConfiguration _config;
    private readonly ILogger<RedisTokenBlacklistService> _logger;
    private readonly string _instanceName;
    private readonly ConnectionMultiplexer? _redis;
    private readonly IDatabase? _db;

    /// <summary>
    /// 内存降级存储（Redis不可用时使用）
    /// </summary>
    private readonly HashSet<(string Token, DateTime ExpireAt)> _memoryBlacklist = new();

    /// <summary>
    /// 内存降级RefreshToken存储
    /// </summary>
    private readonly Dictionary<string, (long UserId, DateTime ExpireAt)> _memoryRefreshTokens = new();

    private readonly object _lock = new();

    /// <summary>
    /// Redis是否可用
    /// </summary>
    private bool _redisAvailable;

    public RedisTokenBlacklistService(IConfiguration config, ILogger<RedisTokenBlacklistService> logger)
    {
        _config = config;
        _logger = logger;
        _instanceName = config["RedisSettings:InstanceName"] ?? "finance:";

        var connectionString = config["RedisSettings:ConnectionString"] ?? "";
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectTimeout = 3000;
                options.AsyncTimeout = 3000;
                _redis = ConnectionMultiplexer.Connect(options);
                _db = _redis.GetDatabase();
                _redisAvailable = true;
                _logger.LogInformation("Token黑名单服务已连接Redis");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis连接失败，Token黑名单将降级为内存存储");
                _redisAvailable = false;
            }
        }
        else
        {
            _logger.LogWarning("Redis未配置连接字符串，Token黑名单将使用内存存储");
            _redisAvailable = false;
        }
    }

    #region ITokenBlacklistService

    /// <inheritdoc/>
    public async Task AddAsync(string token, int expireMinutes)
    {
        if (_redisAvailable && _db != null)
        {
            try
            {
                var key = $"{_instanceName}blacklist:{token}";
                await _db.StringSetAsync(key, "1", TimeSpan.FromMinutes(expireMinutes));
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis写入Token黑名单失败，降级到内存存储");
            }
        }

        lock (_lock)
        {
            _memoryBlacklist.Add((token, DateTime.Now.AddMinutes(expireMinutes)));
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsBlacklistedAsync(string token)
    {
        if (_redisAvailable && _db != null)
        {
            try
            {
                var key = $"{_instanceName}blacklist:{token}";
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis查询Token黑名单失败，降级到内存查询");
            }
        }

        // 内存降级：清理过期项后检查
        lock (_lock)
        {
            CleanupExpiredBlacklist();
            return _memoryBlacklist.Any(b => b.Token == token);
        }
    }

    #endregion

    #region IRefreshTokenStoreService

    /// <inheritdoc/>
    public async Task StoreAsync(string token, long userId, int expireDays)
    {
        if (_redisAvailable && _db != null)
        {
            try
            {
                var key = $"{_instanceName}refresh_token:{token}";
                await _db.StringSetAsync(key, userId.ToString(), TimeSpan.FromDays(expireDays));
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis写入RefreshToken失败，降级到内存存储");
            }
        }

        lock (_lock)
        {
            _memoryRefreshTokens[token] = (userId, DateTime.Now.AddDays(expireDays));
        }
    }

    /// <inheritdoc/>
    public async Task<long?> GetUserIdAsync(string token)
    {
        if (_redisAvailable && _db != null)
        {
            try
            {
                var key = $"{_instanceName}refresh_token:{token}";
                var value = await _db.StringGetAsync(key);
                if (value.HasValue && long.TryParse(value!, out var userId))
                {
                    return userId;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis查询RefreshToken失败，降级到内存查询");
            }
        }

        lock (_lock)
        {
            CleanupExpiredRefreshTokens();
            if (_memoryRefreshTokens.TryGetValue(token, out var entry))
            {
                return entry.UserId;
            }
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string token)
    {
        if (_redisAvailable && _db != null)
        {
            try
            {
                var key = $"{_instanceName}refresh_token:{token}";
                await _db.KeyDeleteAsync(key);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis删除RefreshToken失败，降级到内存操作");
            }
        }

        lock (_lock)
        {
            _memoryRefreshTokens.Remove(token);
        }
    }

    #endregion

    /// <summary>
    /// 清理内存中过期的黑名单项
    /// </summary>
    private void CleanupExpiredBlacklist()
    {
        var now = DateTime.Now;
        _memoryBlacklist.RemoveWhere(b => b.ExpireAt < now);
    }

    /// <summary>
    /// 清理内存中过期的RefreshToken
    /// </summary>
    private void CleanupExpiredRefreshTokens()
    {
        var now = DateTime.Now;
        var expired = _memoryRefreshTokens
            .Where(kv => kv.Value.ExpireAt < now)
            .Select(kv => kv.Key)
            .ToList();
        foreach (var key in expired)
        {
            _memoryRefreshTokens.Remove(key);
        }
    }

    /// <summary>
    /// 释放Redis连接
    /// </summary>
    public void Dispose()
    {
        _redis?.Dispose();
    }
}
