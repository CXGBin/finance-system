using FinanceSystem.Core.Interfaces;
using FinanceSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSystem.Infrastructure;

/// <summary>
/// 仓储层 DI 扩展
/// </summary>
public static class RepositoryServiceExtensions
{
    /// <summary>
    /// 注册泛型仓储（所有实体类共用）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}
