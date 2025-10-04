using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetAvailableOrdersForShipper;

public class GetAvailableOrdersForShipperQuery : IRequest<List<AvailableShipperOrderDto>>
{
    public GetAvailableOrdersForShipperQuery(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public decimal Latitude { get; }
    public decimal Longitude { get; }
}
