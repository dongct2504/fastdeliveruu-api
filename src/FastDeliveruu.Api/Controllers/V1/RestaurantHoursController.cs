using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.RestaurantHourDtos;
using FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;
using FastDeliveruu.Application.RestaurantHours.Commands.DeleteRestaurantHour;
using FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;
using FastDeliveruu.Application.RestaurantHours.Queries.GetByRestaurant;
using FastDeliveruu.Application.RestaurantHours.Queries.GetRestaurantHourById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id:guid}", Name = "GetRestaurantHourById")]
    [ProducesResponseType(typeof(RestaurantHourDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantHourById(Guid id)
    {
        GetRestaurantHourByIdQuery query = new GetRestaurantHourByIdQuery(id);
        Result<RestaurantHourDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(RestaurantHourDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRestaurantHour([FromBody] CreateRestaurantHourCommand command)
    {
        Result<RestaurantHourDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return CreatedAtRoute(
            nameof(GetRestaurantHourById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRestaurantHour(Guid id, [FromBody] UpdateRestaurantHourCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRestaurantHour(Guid id)
    {
        DeleteRestaurantHourCommand command = new DeleteRestaurantHourCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }
}
