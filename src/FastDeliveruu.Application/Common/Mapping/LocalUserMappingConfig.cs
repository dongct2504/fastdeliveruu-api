using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Users.Commands.UpdateUser;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace FastDeliveruu.Application.Common.Mapping;

public class LocalUserMappingConfig : IRegister
{
    private readonly IConfiguration _configuration;

    public LocalUserMappingConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Register(TypeAdapterConfig config)
    {
        string apiUrl = _configuration["ApiUrl"];

        config.NewConfig<LocalUser, LocalUserDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl));

        config.NewConfig<LocalUser, LocalUserDetailDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageUrl.MapImageUrl(apiUrl))
            .Map(dest => dest.OrderDtos, src => src.Orders);

        config.NewConfig<UpdateUserCommand, LocalUser>();
    }
}