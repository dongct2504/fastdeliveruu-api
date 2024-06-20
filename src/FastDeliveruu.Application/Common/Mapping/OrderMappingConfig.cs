using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Orders.Commands.CreateOrder;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.DeliveryMethodShortName, src => src.DeliveryMethod.ShortName)
            .Map(dest => dest.ShippingPrice, src => src.DeliveryMethod.Price);

        config.NewConfig<Order, OrderHeaderDetailDto>()
            .Map(dest => dest.DeliveryMethodDto, src => src.DeliveryMethod)
            .Map(dest => dest.OrderDetailDtos, src => src.OrderDetails);

        config.NewConfig<OrderDetail, OrderDetailDto>()
            .Map(dest => dest.MenuItemDto, src => src.MenuItem);

        config.NewConfig<DeliveryMethod, DeliveryMethodDto>();

        config.NewConfig<CreateOrderCommand, Order>();
    }
}
