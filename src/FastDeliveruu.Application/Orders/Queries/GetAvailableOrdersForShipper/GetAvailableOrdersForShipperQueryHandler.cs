using FastDeliveruu.Common.Enums;
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
            .Where(o => !o.OrderDeliveries.Any() && o.DeliveryMethodId == 1) // only orders with status 1
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
                PaymentMethodText = x.o.PaymentMethod == 1 ? "Tiền mặt" : x.o.PaymentMethod == 2 ? "VNPAY" : x.o.PaymentMethod == 3 ? "PayPal" : null,
                TotalAmount = x.o.TotalAmount,
                OrderStatus = x.o.OrderStatus,
                OrderStatusText = x.o.OrderStatus == 5 ? "Đã thanh toán"
                                  : x.o.OrderStatus == 3 ? "Đã hủy"
                                  : x.o.OrderStatus == 1 ? "Chưa thanh toán"
                                  : x.o.OrderStatus == 2 ? "Đang xử lý"
                                  : x.o.OrderStatus == 4 ? "Thanh toán thất bại"
                                  : x.o.OrderStatus == 6 ? "Đã giao hàng"
                                  : x.o.OrderStatus == 7 ? "Đã hoàn tiền"
                                  : x.o.OrderStatus == 8 ? "Thanh toán chậm" : null,
                DeliveryMethodId = x.o.DeliveryMethodId,
                Latitude = x.o.Latitude,
                Longitude = x.o.Longitude
            });

        return await query.ToListAsync(cancellationToken);
    }
}
