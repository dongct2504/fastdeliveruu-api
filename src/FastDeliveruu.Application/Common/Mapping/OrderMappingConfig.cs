using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Orders.Commands.CreateOrder;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>();

        config.NewConfig<Order, OrderHeaderDetailDto>()
            .Map(dest => dest.LocalUserDto, src => src.LocalUser)
            .Map(dest => dest.ShipperDto, src => src.Shipper)
            .Map(dest => dest.OrderDetailDtos, src => src.OrderDetails);

        config.NewConfig<OrderDetail, OrderDetailDto>();

        config.NewConfig<CreateOrderCommand, Order>();
    }
}
