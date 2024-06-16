using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetDeliveryMethods;

public class GetDeliveryMethodsQuery : IRequest<List<DeliveryMethodDto>>
{
}
