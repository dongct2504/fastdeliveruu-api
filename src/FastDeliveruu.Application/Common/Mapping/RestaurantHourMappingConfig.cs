using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;
using FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class RestaurantHourMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RestaurantHour, RestaurantHourDto>();

        config.NewConfig<CreateRestaurantHourCommand, RestaurantHour>();

        config.NewConfig<UpdateRestaurantHourCommand, RestaurantHour>();
    }
}
