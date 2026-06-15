using SqlSugar;

namespace FinanceSystem.Infrastructure.Configuration;

/// <summary>
/// 数据库配置选项
/// </summary>
public class DbSettings
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型（SqlServer、MySQL、Sqlite）
    /// </summary>
    public string DbType { get; set; } = "SqlServer";

    /// <summary>
    /// 是否自动生成数据库表结构（开发环境建议开启）
    /// </summary>
    public bool AutoCreateTable { get; set; } = true;
}

/// <summary>
/// JWT 配置选项
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 发行方
    /// </summary>
    public string Issuer { get; set; } = "FinanceSystem";

    /// <summary>
    /// 接收方
    /// </summary>
    public string Audience { get; set; } = "FinanceSystem";

    /// <summary>
    /// Access Token 有效期（分钟）
    /// </summary>
    public int AccessTokenExpireMinutes { get; set; } = 120;

    /// <summary>
    /// Refresh Token 有效期（天）
    /// </summary>
    public int RefreshTokenExpireDays { get; set; } = 7;
}

/// <summary>
/// Redis 配置选项
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 实例名称（Key前缀）
    /// </summary>
    public string InstanceName { get; set; } = "finance:";
}
