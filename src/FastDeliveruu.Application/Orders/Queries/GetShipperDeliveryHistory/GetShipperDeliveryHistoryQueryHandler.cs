using FastDeliveruu.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetShipperDeliveryHistory;

public class GetShipperDeliveryHistoryQueryHandler : IRequestHandler<GetShipperDeliveryHistoryQuery, List<ShipperDeliveryHistoryDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;

    public GetShipperDeliveryHistoryQueryHandler(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShipperDeliveryHistoryDto>> Handle(GetShipperDeliveryHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.OrderDeliveries
            .AsNoTracking()
            .Where(d => d.ShipperId == request.ShipperId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new ShipperDeliveryHistoryDto
            {
                Id = d.Id,
                OrderId = d.OrderId,
                ShipperId = d.ShipperId.ToString(),
                DeliveryStatus = d.DeliveryStatus,
                EstimatedDeliveryTime = d.EstimatedDeliveryTime,
                ActualDeliveryTime = d.ActualDeliveryTime,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}