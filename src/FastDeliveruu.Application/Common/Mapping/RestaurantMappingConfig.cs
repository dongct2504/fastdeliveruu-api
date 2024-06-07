using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;
using FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace FastDeliveruu.Application.Common.Mapping;

public class RestaurantMappingConfig : IRegister
{
    private readonly IConfiguration _configuration;

    public RestaurantMappingConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Register(TypeAdapterConfig config)
    {
        string apiUrl = _configuration["ApiUrl"];

        config.NewConfig<Restaurant, RestaurantDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl));

        config.NewConfig<Restaurant, RestaurantDetailDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl))
            .Map(dest => dest.MenuItemDtos, src => src.MenuItems);

        config.NewConfig<CreateRestaurantCommand, Restaurant>();

        config.NewConfig<UpdateRestaurantCommand, Restaurant>();
    }
}