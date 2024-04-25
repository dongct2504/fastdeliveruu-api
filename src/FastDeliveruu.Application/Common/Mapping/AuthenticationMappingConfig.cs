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
        config.NewConfig<RegisterationRequestDto, RegisterCommand>();

        config.NewConfig<LoginRequestDto, LoginQuery>();

        config.NewConfig<(LocalUser localUser, string token), AuthenticationResponse>()
            .Map(dest => dest.Token, src => src.token)
            .Map(dest => dest.LocalUserDto, src => src.localUser);

        config.NewConfig<RegisterationShipperDto, RegisterShipperCommand>();

        config.NewConfig<LoginShipperDto, LoginShipperQuery>();

        config.NewConfig<(Shipper shipper, string token), AuthenticationShipperResponse>()
            .Map(dest => dest.Token, src => src.token)
            .Map(dest => dest.ShipperDto, src => src.token);
    }
}