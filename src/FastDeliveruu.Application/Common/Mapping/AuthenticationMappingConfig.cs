using FastDeliveruu.Application.Authentication.Commands.Register;
using FastDeliveruu.Application.Authentication.Commands.RegisterShipper;
using FastDeliveruu.Application.Authentication.Queries.Login;
using FastDeliveruu.Application.Authentication.Queries.LoginShipper;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(LocalUser localUser, string token), AuthenticationResponse>()
            .Map(dest => dest.LocalUserDto, src => src.localUser)
            .Map(dest => dest.Token, src => src.token);

        config.NewConfig<(Shipper shipper, string token), AuthenticationShipperResponse>()
            .Map(dest => dest.ShipperDto, src => src.shipper)
            .Map(dest => dest.Token, src => src.token);
    }
}