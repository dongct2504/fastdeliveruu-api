using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(AppUser appUser, string token), UserAuthenticationResponse>()
            .Map(dest => dest.AppUserDto, src => src.appUser)
            .Map(dest => dest.Token, src => src.token);

        config.NewConfig<(Shipper shipper, string token), ShipperAuthenticationResponse>()
            .Map(dest => dest.ShipperDto, src => src.shipper)
            .Map(dest => dest.Token, src => src.token);
    }
}