using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShoppingCartMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ShoppingCart, ShoppingCartDto>()
            .Map(dest => dest.MenuItemDto, src => src.MenuItem)
            .Map(dest => dest.MenuVariantDto, src => src.MenuVariant);

        config.NewConfig<UpdateCartItemCommand, ShoppingCart>();
    }
}
