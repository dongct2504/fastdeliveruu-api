using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Authentication;
using FastDeliveruu.Infrastructure.Repositories;
using FastDeliveruu.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<ISP_Call, SP_Call>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<ILocalUserRepository, LocalUserRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        return services;
    }
}
