using SqlSugar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinanceSystem.Infrastructure.Configuration;

namespace FinanceSystem.Infrastructure.Extensions;

/// <summary>
/// SqlSugar 数据库 DI 扩展
/// </summary>
public static class SqlSugarExtensions
{
    /// <summary>
    /// 注册 SqlSugar ORM 服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置根</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqlSugarSetup(this IServiceCollection services, IConfiguration configuration)
    {
        // 从配置读取数据库设置
        var settings = configuration.GetSection("DbSettings").Get<DbSettings>() ?? new DbSettings();

        // 解析数据库类型
        var dbType = settings.DbType?.ToLower() switch
        {
            "sqlserver" => DbType.SqlServer,
            "mysql" => DbType.MySql,
            "sqlite" => DbType.Sqlite,
            _ => DbType.SqlServer
        };

        // 注册 SqlSugar 单例
        services.AddSingleton<ISqlSugarClient>(sp =>
        {
            var db = new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = settings.ConnectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            },
            db =>
            {
                // 开发环境打印SQL
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine($"[SQL] {sql}");
                };
            });

            return db;
        });

        return services;
    }
}
