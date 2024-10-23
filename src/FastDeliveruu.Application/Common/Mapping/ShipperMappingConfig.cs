using FastDeliveruu.Application.Authentication.Commands.ShipperRegister;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShipperMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Shipper, ShipperDto>();

        config.NewConfig<ShipperRegisterCommand, Shipper>();
    }
}
