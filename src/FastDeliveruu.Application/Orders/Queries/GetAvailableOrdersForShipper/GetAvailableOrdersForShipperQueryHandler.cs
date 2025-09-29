using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetAvailableOrdersForShipper;

public class GetAvailableOrdersForShipperQueryHandler : IRequestHandler<GetAvailableOrdersForShipperQuery, List<AvailableShipperOrderDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAvailableOrdersForShipperQueryHandler(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AvailableShipperOrderDto>> Handle(GetAvailableOrdersForShipperQuery request, CancellationToken cancellationToken)
    {
        decimal lat = request.Latitude;
        decimal lng = request.Longitude;

        var query = _dbContext.Orders
            .AsNoTracking()
            .Where(o => !o.OrderDeliveries.Any()) // no shipper accepted yet
            .Select(o => new
            {
                o,
                Distance = ((o.Latitude - lat) * (o.Latitude - lat)) + ((o.Longitude - lng) * (o.Longitude - lng))
            })
            .OrderBy(x => x.Distance)
            .Select(x => new AvailableShipperOrderDto
            {
                Id = x.o.Id,
                OrderDate = x.o.OrderDate,
                PaymentMethod = x.o.PaymentMethod,
                PaymentMethodText = x.o.PaymentMethod == 1 ? "Ti?n m?t" : x.o.PaymentMethod == 2 ? "VNPAY" : x.o.PaymentMethod == 3 ? "PayPal" : null,
                TotalAmount = x.o.TotalAmount,
                OrderStatus = x.o.OrderStatus,
                OrderStatusText = x.o.OrderStatus == 5 ? "?� thanh to�n"
                                  : x.o.OrderStatus == 3 ? "?� h?y"
                                  : x.o.OrderStatus == 1 ? "Ch?a thanh to�n"
                                  : x.o.OrderStatus == 2 ? "?ang x? l�"
                                  : x.o.OrderStatus == 4 ? "Thanh to�n th?t b?i"
                                  : x.o.OrderStatus == 6 ? "?� giao h�ng"
                                  : x.o.OrderStatus == 7 ? "?� ho�n ti?n"
                                  : x.o.OrderStatus == 8 ? "Thanh to�n ch?m" : null,
                Latitude = x.o.Latitude,
                Longitude = x.o.Longitude
            });

        return await query.ToListAsync(cancellationToken);
    }
}
