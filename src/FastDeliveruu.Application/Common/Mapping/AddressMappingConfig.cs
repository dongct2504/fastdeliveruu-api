using FastDeliveruu.Application.Cities.Commands.CreateCity;
using FastDeliveruu.Application.Cities.Commands.UpdateCity;
using FastDeliveruu.Application.Dtos.AddressDtos;
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

        config.NewConfig<Ward, WardDto>();
    }
}
