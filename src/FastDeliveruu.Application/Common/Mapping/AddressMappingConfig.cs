using FastDeliveruu.Application.Cities.Commands.CreateCity;
using FastDeliveruu.Application.Cities.Commands.UpdateCity;
using FastDeliveruu.Application.Districts.Commands.CreateDistrict;
using FastDeliveruu.Application.Districts.Commands.UpdateDistrict;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Wards.Commands.CreateWard;
using FastDeliveruu.Application.Wards.Commands.UpdateWard;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class AddressMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<City, CityDto>();
        config.NewConfig<City, CityDetailDto>()
            .Map(dest => dest.DistrictDtos, src => src.Districts);
        config.NewConfig<CreateCityCommand, City>();
        config.NewConfig<UpdateCityCommand, City>();

        config.NewConfig<District, DistrictDto>();
        config.NewConfig<District, DistrictDetailDto>()
            .Map(dest => dest.WardDtos, src => src.Wards);
        config.NewConfig<CreateDistrictCommand, District>();
        config.NewConfig<UpdateDistrictCommand, District>();

        config.NewConfig<Ward, WardDto>();
        config.NewConfig<CreateWardCommand, Ward>();
        config.NewConfig<UpdateWardCommand, Ward>();
    }
}
