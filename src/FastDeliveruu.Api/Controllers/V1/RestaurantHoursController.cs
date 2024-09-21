using Asp.Versioning;
using FastDeliveruu.Application.Dtos.RestaurantHourDtos;
using FastDeliveruu.Application.RestaurantHours.Queries.GetByRestaurant;
using FastDeliveruu.Application.RestaurantHours.Queries.GetRestaurantHourById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("get-by-restaurant/{restaurantId:guid}")]
    [ProducesResponseType(typeof(List<RestaurantHourDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId)
    {
        GetByRestaurantQuery query = new GetByRestaurantQuery(restaurantId);
        List<RestaurantHourDto> restaurantHourDtos = await _mediator.Send(query);
        return Ok(restaurantHourDtos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RestaurantHourDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetRestaurantHourByIdQuery query = new GetRestaurantHourByIdQuery(id);
        Result<RestaurantHourDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }
}
