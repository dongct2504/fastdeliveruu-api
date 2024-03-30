using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISP_Call, SP_Call>();

        return services;
    }
}
