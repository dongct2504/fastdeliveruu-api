using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShipperMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Shipper, ShipperDto>();

        config.NewConfig<Shipper, ShipperDetailDto>()
            .Map(dest => dest.OrderDtos, src => src.Orders);
    }
}