using Asp.Versioning;
using FastDeliveruu.Application.Orders.Queries.GetAvailableOrdersForShipper;
using FastDeliveruu.Application.Orders.Queries.GetShipperDeliveryHistory;
using FastDeliveruu.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shipper-orders")]
public class ShipperOrdersController : ApiController
{
    private readonly IMediator _mediator;

    public ShipperOrdersController(IMediator mediator)
    {
        _mediator = mediator;
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
}
