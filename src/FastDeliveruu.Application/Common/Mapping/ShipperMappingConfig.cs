using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Shippers.Commands.UpdateShipper;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace FastDeliveruu.Application.Common.Mapping;

public class ShipperMappingConfig : IRegister
{
    private readonly IConfiguration _configuration;

    public ShipperMappingConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Register(TypeAdapterConfig config)
    {
        string apiUrl = _configuration["ApiUrl"];

        config.NewConfig<Shipper, ShipperDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl));

        config.NewConfig<Shipper, ShipperDetailDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl))
            .Map(dest => dest.OrderDtos, src => src.Orders);

        config.NewConfig<UpdateShipperCommand, Shipper>();
    }
}