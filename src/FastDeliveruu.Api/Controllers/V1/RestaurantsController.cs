using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Common.Constants;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FastDeliveruu.Application.Dtos;
using MediatR;
using FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;
using FastDeliveruu.Application.Restaurants.Queries.GetRestaurantById;
using FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;
using FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;
using FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;
using Asp.Versioning;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/restaurants")]
public class RestaurantsController : ApiController
{
    private readonly IMediator _mediator;

    public RestaurantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<RestaurantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRestaurants([FromQuery] RestaurantParams restaurantParams)
    {
        GetAllRestaurantsQuery query = new GetAllRestaurantsQuery(restaurantParams);
        PagedList<RestaurantDto> paginationResponse = await _mediator.Send(query);
        return Ok(paginationResponse);
    }

    [HttpGet("{id:guid}", Name = "GetRestaurantById")]
    [ProducesResponseType(typeof(RestaurantDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantById(Guid id)
    {
        GetRestaurantByIdQuery query = new GetRestaurantByIdQuery(id);
        Result<RestaurantDetailDto> getRestaurantResult = await _mediator.Send(query);
        if (getRestaurantResult.IsFailed)
        {
            return Problem(getRestaurantResult.Errors);
        }

        return Ok(getRestaurantResult.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantCommand command)
    {
        Result<RestaurantDto> createRestaurantResult = await _mediator.Send(command);
        if (createRestaurantResult.IsFailed)
        {
            return Problem(createRestaurantResult.Errors);
        }

        return CreatedAtRoute(
            nameof(GetRestaurantById),
            new { id = createRestaurantResult.Value.Id },
            createRestaurantResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRestaurant(Guid id, [FromForm] UpdateRestaurantCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updateRestaurantResult = await _mediator.Send(command);
        if (updateRestaurantResult.IsFailed)
        {
            return Problem(updateRestaurantResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRestaurant(Guid id)
    {
        DeleteRestaurantCommand command = new DeleteRestaurantCommand(id);
        Result deleteRestaurantResult = await _mediator.Send(command);
        if (deleteRestaurantResult.IsFailed)
        {
            return Problem(deleteRestaurantResult.Errors);
        }

        return NoContent();
    }
}