using FastDeliveruu.Application.Authentication.Commands.UserRegister;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AppUserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, AppUserDto>()
            .Map(dest => dest.Address,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Address)
                    .FirstOrDefault())
            .Map(dest => dest.CityId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.CityId)
                    .FirstOrDefault())
            .Map(dest => dest.DistrictId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.DistrictId)
                    .FirstOrDefault())
            .Map(dest => dest.WardId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.WardId)
                    .FirstOrDefault())
            .Map(dest => dest.Latitude,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Latitude)
                    .FirstOrDefault())
            .Map(dest => dest.Longitude,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Longitude)
                    .FirstOrDefault());

        config.NewConfig<AppUser, AppUserDetailDto>()
            .Map(dest => dest.Address,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Address)
                    .FirstOrDefault())
            .Map(dest => dest.CityId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.CityId)
                    .FirstOrDefault())
            .Map(dest => dest.DistrictId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.DistrictId)
                    .FirstOrDefault())
            .Map(dest => dest.WardId,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.WardId)
                    .FirstOrDefault())
            .Map(dest => dest.Latitude,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Latitude)
                    .FirstOrDefault())
            .Map(dest => dest.Longitude,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Longitude)
                    .FirstOrDefault());

        config.NewConfig<AppUser, AppUserWithRolesDto>()
            .Map(dest => dest.Roles, src => src.AppUserRoles.Select(ur => ur.AppRole.Name).ToList());

        config.NewConfig<UserRegisterCommand, AppUser>();
    }
}
