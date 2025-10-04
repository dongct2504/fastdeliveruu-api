using MediatR;
using System.Collections.Generic;
using System;

namespace FastDeliveruu.Application.Orders.Queries.GetShipperDeliveryHistory;

public class GetShipperDeliveryHistoryQuery : IRequest<List<ShipperDeliveryHistoryDto>>
{
    public GetShipperDeliveryHistoryQuery(Guid shipperId)
    {
        ShipperId = shipperId;
    }

    public Guid ShipperId { get; }
}