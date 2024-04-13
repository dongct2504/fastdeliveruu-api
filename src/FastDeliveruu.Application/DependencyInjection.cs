using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGenreServices, GenreServices>();
        services.AddScoped<IRestaurantServices, RestaurantServices>();
        services.AddScoped<IMenuItemServices, MenuItemServices>();
        services.AddScoped<ILocalUserServices, localUserServices>();
        services.AddScoped<IAuthenticationServices, AuthenticationServices>();
        services.AddScoped<IShoppingCartServices, ShoppingCartServices>();

        return services;
    }
}
