using FastDeliveruu.Application.Authentication.Commands.Register;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AppUserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, AppUserDto>()
            .Map(dest => dest.Address, src => src.AddressesCustomers.FirstOrDefault(ac => ac.IsPrimary).Address)
            .Map(dest => dest.City, src => src.AddressesCustomers.FirstOrDefault(ac => ac.IsPrimary).City.Name)
            .Map(dest => dest.District, src => src.AddressesCustomers.FirstOrDefault(ac => ac.IsPrimary).District.Name)
            .Map(dest => dest.Ward, src => src.AddressesCustomers.FirstOrDefault(ac => ac.IsPrimary).Ward.Name);

        config.NewConfig<AppUser, AppUserDetailDto>();

        config.NewConfig<RegisterCommand, AppUser>();
    }
}
