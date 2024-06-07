using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastDeliveruu.Application.Common.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddMappings(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;

        //config.Scan(Assembly.GetExecutingAssembly());

        GenreMappingConfig genreMappingConfig = new GenreMappingConfig();
        genreMappingConfig.Register(config);

        RestaurantMappingConfig restaurantMappingConfig = new RestaurantMappingConfig(configuration);
        restaurantMappingConfig.Register(config);

        MenuItemMappingConfig menuItemMappingConfig = new MenuItemMappingConfig(configuration);
        menuItemMappingConfig.Register(config);

        AuthenticationMappingConfig authenticationMappingConfig = new AuthenticationMappingConfig();
        authenticationMappingConfig.Register(config);

        LocalUserMappingConfig localUserMappingConfig = new LocalUserMappingConfig(configuration);
        localUserMappingConfig.Register(config);

        ShipperMappingConfig shipperMappingConfig = new ShipperMappingConfig(configuration);
        shipperMappingConfig.Register(config);

        ShoppingCartMappingConfig shoppingCartMappingConfig = new ShoppingCartMappingConfig();
        shoppingCartMappingConfig.Register(config);

        OrderMappingConfig orderMappingConfig = new OrderMappingConfig();
        orderMappingConfig.Register(config);

        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}