using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(AppUser appUser, string token), AuthenticationResponse>()
            .Map(dest => dest.AppUserDto, src => src.appUser)
            .Map(dest => dest.Token, src => src.token);
    }
}