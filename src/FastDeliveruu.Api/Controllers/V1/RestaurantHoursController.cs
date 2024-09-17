using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/restaurant-hours")]
public class RestaurantHoursController : ApiController
{
    private readonly IMediator _mediator;

    public RestaurantHoursController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
