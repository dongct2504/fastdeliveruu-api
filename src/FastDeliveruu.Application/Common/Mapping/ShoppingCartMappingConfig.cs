using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.ShoppingCarts.Commands.CreateShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShoppingCartMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ShoppingCart, ShoppingCartDto>()
            .Map(dest => dest.MenuItemDto, src => src.MenuItem);

        config.NewConfig<CreateShoppingCartCommand, ShoppingCart>();

        config.NewConfig<UpdateShoppingCartCommand, ShoppingCart>();
    }
}
