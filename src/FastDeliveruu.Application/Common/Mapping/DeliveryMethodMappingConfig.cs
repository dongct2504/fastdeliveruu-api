using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class DeliveryMethodMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DeliveryMethod, DeliveryMethodDto>();
    }
}
