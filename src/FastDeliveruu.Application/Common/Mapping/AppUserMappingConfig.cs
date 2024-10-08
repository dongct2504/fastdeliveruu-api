﻿using FastDeliveruu.Application.Authentication.Commands.UserRegister;
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
            .Map(dest => dest.City,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.City.Name)
                    .FirstOrDefault())
            .Map(dest => dest.District,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.District.Name)
                    .FirstOrDefault())
            .Map(dest => dest.Ward,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Ward.Name)
                    .FirstOrDefault());

        config.NewConfig<AppUser, AppUserDetailDto>()
            .Map(dest => dest.Address,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Address)
                    .FirstOrDefault())
            .Map(dest => dest.City,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.City.Name)
                    .FirstOrDefault())
            .Map(dest => dest.District,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.District.Name)
                    .FirstOrDefault())
            .Map(dest => dest.Ward,
                src => src.AddressesCustomers
                    .Where(ac => ac.IsPrimary)
                    .Select(ac => ac.Ward.Name)
                    .FirstOrDefault());

        config.NewConfig<AppUser, AppUserWithRolesDto>()
            .Map(dest => dest.Roles, src => src.AppUserRoles.Select(ur => ur.AppRole.Name).ToList());

        config.NewConfig<UserRegisterCommand, AppUser>();
    }
}
