using FastDeliveruu.Application.Authentication.Commands.ShipperRegister;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShipperMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Shipper, ShipperDto>()
            .Map(dest => dest.City, src => src.City.Name)
            .Map(dest => dest.District, src => src.District.Name)
            .Map(dest => dest.Ward, src => src.Ward.Name);

        config.NewConfig<ShipperRegisterCommand, Shipper>();
    }
}
