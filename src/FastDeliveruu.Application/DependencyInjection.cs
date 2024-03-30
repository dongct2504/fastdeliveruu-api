using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGenreServices, GenreServices>();

        return services;
    }
}
