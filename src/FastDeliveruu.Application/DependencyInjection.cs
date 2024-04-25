using System.Reflection;
using FastDeliveruu.Application.Common.Behaviors;
using FastDeliveruu.Application.Common.Mapping;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMappings();

        services.AddMediatR(typeof(DependencyInjection).Assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IGenreServices, GenreServices>();
        services.AddScoped<IRestaurantServices, RestaurantServices>();
        services.AddScoped<IMenuItemServices, MenuItemServices>();
        services.AddScoped<ILocalUserServices, localUserServices>();
        services.AddScoped<IShoppingCartServices, ShoppingCartServices>();
        services.AddScoped<IOrderServices, OrderServices>();

        return services;
    }
}
