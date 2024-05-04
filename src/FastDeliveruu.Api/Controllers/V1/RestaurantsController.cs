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
    [ResponseCache(CacheProfileName = CacheProfileConstants.Default30, VaryByQueryKeys = new[] { "page" })]
    [ProducesResponseType(typeof(PaginationResponse<RestaurantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRestaurants(int page = 1)
    {
        try
        {
            GetAllRestaurantsQuery query = new GetAllRestaurantsQuery(page);
            PaginationResponse<RestaurantDto> paginationResponse = await _mediator.Send(query);
            return Ok(paginationResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id:int}", Name = "GetRestaurantById")]
    [ProducesResponseType(typeof(RestaurantDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantById(int id)
    {
        try
        {
            GetRestaurantByIdQuery query = new GetRestaurantByIdQuery(id);
            Result<RestaurantDetailDto> getRestaurantResult = await _mediator.Send(query);
            if (getRestaurantResult.IsFailed)
            {
                return Problem(getRestaurantResult.Errors);
            }

            return Ok(getRestaurantResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    //[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantCommand command)
    {
        try
        {
            Result<RestaurantDto> createRestaurantResult = await _mediator.Send(command);
            if (createRestaurantResult.IsFailed)
            {
                return Problem(createRestaurantResult.Errors);
            }

            return CreatedAtRoute(
                nameof(GetRestaurantById),
                new { Id = createRestaurantResult.Value.RestaurantId },
                createRestaurantResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id:int}")]
    //[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromForm] UpdateRestaurantCommand command)
    {
        try
        {
            if (id != command.RestaurantId)
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
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        try
        {
            DeleteRestaurantCommand command = new DeleteRestaurantCommand(id);
            Result deleteRestaurantResult = await _mediator.Send(command);
            if (deleteRestaurantResult.IsFailed)
            {
                return Problem(deleteRestaurantResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}