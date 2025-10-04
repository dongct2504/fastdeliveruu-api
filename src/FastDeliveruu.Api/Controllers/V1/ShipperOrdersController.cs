using Asp.Versioning;
using FastDeliveruu.Application.Orders.Queries.GetAvailableOrdersForShipper;
using FastDeliveruu.Application.Orders.Queries.GetShipperDeliveryHistory;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shipper-orders")]
public class ShipperOrdersController : ApiController
{
    private readonly IMediator _mediator;
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Random _random = new Random();

    public ShipperOrdersController(IMediator mediator, FastDeliveruuDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _mediator = mediator;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable([FromQuery] decimal lat, [FromQuery] decimal lng)
    {
        var query = new GetAvailableOrdersForShipperQuery(lat, lng);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<ShipperDeliveryHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory()
    {
        var shipperId = User.GetCurrentUserId(); // Guid
        var result = await _mediator.Send(new GetShipperDeliveryHistoryQuery(shipperId));
        return Ok(result);
    }

    public record AcceptOrderRequest(Guid OrderId);

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOrder([FromBody] AcceptOrderRequest request)
    {
        var shipperId = User.GetCurrentUserId();
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.OrderId);
        if (order == null) return NotFound("Không tìm th?y ??n Hàng");

        bool hasDelivery = await _dbContext.OrderDeliveries.AnyAsync(od => od.OrderId == order.Id);
        if (hasDelivery) return BadRequest("??n hàng ?ã ???c nh?n b?i shipper khác");

        var shipper = await _dbContext.Shippers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == shipperId);
        if (shipper == null) return Unauthorized();

        // compute distance (meters) via Haversine
        double distanceMeters = HaversineInMeters((double)shipper.Latitude, (double)shipper.Longitude, (double)order.Latitude, (double)order.Longitude);
        double speed = 4.0 + _random.NextDouble() * (7.0 - 4.0); // m/s in [4,7]
        double seconds = speed <= 0 ? 0 : distanceMeters / speed;

        DateTime now = _dateTimeProvider.VietnamDateTimeNow;
        DateTime eta = now.AddSeconds(seconds);

        // CreatedAt from OrderDetails.CreatedAt (min) for this OrderId, fallback order.CreatedAt or now
        DateTime createdAt = await _dbContext.OrderDetails
            .Where(od => od.OrderId == order.Id && od.CreatedAt != null)
            .Select(od => od.CreatedAt!.Value)
            .OrderBy(dt => dt)
            .FirstOrDefaultAsync();
        if (createdAt == default) createdAt = order.CreatedAt ?? now;

        var delivery = new OrderDelivery
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ShipperId = shipper.Id,
            DeliveryStatus = 1, // ?ang giao hàng
            EstimatedDeliveryTime = eta,
            ActualDeliveryTime = null,
            CreatedAt = createdAt,
            UpdatedAt = now
        };

        _dbContext.OrderDeliveries.Add(delivery);
        await _dbContext.SaveChangesAsync();
        return Ok(new
        {
            delivery.Id,
            delivery.OrderId,
            delivery.ShipperId,
            delivery.DeliveryStatus,
            delivery.EstimatedDeliveryTime,
            delivery.ActualDeliveryTime,
            delivery.CreatedAt,
            delivery.UpdatedAt
        });
    }

    [HttpPost("{deliveryId:guid}/deliver")]
    public async Task<IActionResult> MarkDelivered([FromRoute] Guid deliveryId)
    {
        var shipperId = User.GetCurrentUserId();
        var delivery = await _dbContext.OrderDeliveries.FirstOrDefaultAsync(d => d.Id == deliveryId && d.ShipperId == shipperId);
        if (delivery == null) return NotFound("Không tìm th?y b?n ghi giao hàng");

        DateTime now = _dateTimeProvider.VietnamDateTimeNow;
        delivery.DeliveryStatus = 2; // ?ã giao
        delivery.ActualDeliveryTime = now;
        delivery.UpdatedAt = now;

        await _dbContext.SaveChangesAsync();
        return Ok(new
        {
            delivery.Id,
            delivery.OrderId,
            delivery.ShipperId,
            delivery.DeliveryStatus,
            delivery.EstimatedDeliveryTime,
            delivery.ActualDeliveryTime,
            delivery.CreatedAt,
            delivery.UpdatedAt
        });
    }

    [HttpDelete("{deliveryId:guid}")]
    public async Task<IActionResult> CancelDelivery([FromRoute] Guid deliveryId)
    {
        var shipperId = User.GetCurrentUserId();
        var delivery = await _dbContext.OrderDeliveries.FirstOrDefaultAsync(d => d.Id == deliveryId && d.ShipperId == shipperId);
        if (delivery == null) return NotFound("Không tìm th?y b?n ghi giao hàng");

        DateTime now = _dateTimeProvider.VietnamDateTimeNow;

        // Update OrderDetails.UpdatedAt for the corresponding OrderId
        var orderDetails = await _dbContext.OrderDetails.Where(od => od.OrderId == delivery.OrderId).ToListAsync();
        foreach (var od in orderDetails)
        {
            od.UpdatedAt = now;
        }

        _dbContext.OrderDeliveries.Remove(delivery);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static double HaversineInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // meters
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double value) => value * Math.PI / 180;
}
