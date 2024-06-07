using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace FastDeliveruu.Application.Common.Mapping;

public class MenuItemMappingConfig : IRegister
{
    private readonly IConfiguration _configuration;

    public MenuItemMappingConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Register(TypeAdapterConfig config)
    {
        string apiUrl = _configuration["ApiUrl"];

        config.NewConfig<MenuItem, MenuItemDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl));

        config.NewConfig<MenuItem, MenuItemDetailDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl))
            .Map(dest => dest.GenreDto, src => src.Genre)
            .Map(dest => dest.RestaurantDto, src => src.Restaurant);

        config.NewConfig<CreateMenuItemCommand, MenuItem>();

        config.NewConfig<UpdateMenuItemCommand, MenuItem>();
    }
}
