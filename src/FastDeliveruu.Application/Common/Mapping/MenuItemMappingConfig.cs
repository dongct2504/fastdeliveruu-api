using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItemInventories.Commands.CreateMenuItemInventory;
using FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;
using FastDeliveruu.Application.MenuVariantInventories.Commands.UpdateMenuVariantInventory;
using FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class MenuItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // MenuItem config
        config.NewConfig<MenuItem, MenuItemDto>();
        config.NewConfig<MenuItem, MenuItemDetailDto>()
            .Map(dest => dest.GenreDto, src => src.Genre)
            .Map(dest => dest.RestaurantDto, src => src.Restaurant);
        config.NewConfig<CreateMenuItemCommand, MenuItem>();
        config.NewConfig<UpdateMenuItemCommand, MenuItem>();

        // MenuVariant config
        config.NewConfig<MenuVariant, MenuVariantDto>();
        config.NewConfig<CreateMenuVariantCommand, MenuVariant>();
        config.NewConfig<UpdateMenuVariantCommand, MenuVariant>();

        // MenuItemInventory config
        config.NewConfig<MenuItemInventory, MenuItemInventoryDto>();
        config.NewConfig<UpdateMenuItemInventoryCommand, MenuItemInventory>();

        // MenuVariantInventory config
        config.NewConfig<MenuVariantInventory, MenuVariantInventoryDto>();
        config.NewConfig<UpdateMenuVariantInventoryCommand, MenuVariantInventory>();
    }
}
