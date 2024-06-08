using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;
using FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class RestaurantMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Restaurant, RestaurantDto>();

        config.NewConfig<Restaurant, RestaurantDetailDto>()
            .Map(dest => dest.MenuItemDtos, src => src.MenuItems);

        config.NewConfig<CreateRestaurantCommand, Restaurant>();

        config.NewConfig<UpdateRestaurantCommand, Restaurant>();
    }
}