using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class MenuItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MenuItem, MenuItemDto>();

        config.NewConfig<MenuItem, MenuItemDetailDto>()
            .Map(dest => dest.GenreDto, src => src.Genre)
            .Map(dest => dest.RestaurantDto, src => src.Restaurant);

        config.NewConfig<CreateMenuItemCommand, MenuItem>();

        config.NewConfig<UpdateMenuItemCommand, MenuItem>();
    }
}
