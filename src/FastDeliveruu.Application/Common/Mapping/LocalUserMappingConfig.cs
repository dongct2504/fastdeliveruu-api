using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Users.Commands.UpdateUser;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class LocalUserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LocalUser, LocalUserDto>();

        config.NewConfig<LocalUser, LocalUserDetailDto>()
            .Map(dest => dest.OrderDtos, src => src.Orders);

        config.NewConfig<UpdateUserCommand, LocalUser>();
    }
}