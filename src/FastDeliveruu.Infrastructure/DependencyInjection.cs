using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISP_Call, SP_Call>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<ILocalUserRepository, LocalUserRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        return services;
    }
}
