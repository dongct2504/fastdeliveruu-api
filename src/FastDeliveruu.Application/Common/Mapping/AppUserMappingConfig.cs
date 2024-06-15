using FastDeliveruu.Application.Authentication.Commands.Register;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AppUserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, AppUserDto>();

        config.NewConfig<AppUser, AppUserDetailDto>();

        config.NewConfig<RegisterCommand, AppUser>();
    }
}
