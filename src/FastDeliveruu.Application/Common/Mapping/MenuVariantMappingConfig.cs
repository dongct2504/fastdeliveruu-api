using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class MenuVariantMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MenuVariant, MenuVariantDto>();

        config.NewConfig<CreateMenuVariantCommand, MenuVariant>();

        config.NewConfig<UpdateMenuVariantCommand, MenuVariant>();
    }
}
