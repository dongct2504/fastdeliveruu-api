using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;
using FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;
using FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;
using FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class RestaurantMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Restaurant config
        config.NewConfig<Restaurant, RestaurantDto>();
        config.NewConfig<Restaurant, RestaurantDetailDto>()
            .Map(dest => dest.MenuItemDtos, src => src.MenuItems)
            .Map(dest => dest.RestaurantHourDtos, src => src.RestaurantHours);
        config.NewConfig<CreateRestaurantCommand, Restaurant>();
        config.NewConfig<UpdateRestaurantCommand, Restaurant>();

        // RestaurantHour config
        config.NewConfig<RestaurantHour, RestaurantHourDto>();
        config.NewConfig<CreateRestaurantHourCommand, RestaurantHour>();
        config.NewConfig<UpdateRestaurantHourCommand, RestaurantHour>();
    }
}