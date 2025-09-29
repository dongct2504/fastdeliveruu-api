using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentDto>();
    }
}
