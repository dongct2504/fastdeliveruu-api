using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Shippers.Commands.UpdateShipper;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShipperMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Shipper, ShipperDto>();

        config.NewConfig<UpdateShipperCommand, Shipper>();
    }
}