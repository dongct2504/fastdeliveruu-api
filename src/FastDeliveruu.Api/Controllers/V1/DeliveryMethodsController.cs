using Asp.Versioning;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Orders.Queries.GetDeliveryMethods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/delivery-methods")]
public class DeliveryMethodsController : ApiController
{
    private readonly IMediator _mediator;

    public DeliveryMethodsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DeliveryMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeliveryMethods()
    {
        GetDeliveryMethodsQuery query = new GetDeliveryMethodsQuery();
        List<DeliveryMethodDto> deliveryMethodDtos = await _mediator.Send(query);
        return Ok(deliveryMethodDtos);
    }
}
