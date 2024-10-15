using FastDeliveruu.Application.Dtos.WishListDtos;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class WishlistMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WishList, WishListDto>()
            .Map(dest => dest.MenuItemDto, src => src.MenuItem)
            .Map(dest => dest.MenuVariantDto, src => src.MenuVariant);
    }
}
