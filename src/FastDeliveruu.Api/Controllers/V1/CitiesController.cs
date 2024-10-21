using Asp.Versioning;
using FastDeliveruu.Application.Cities.Commands.CreateCity;
using FastDeliveruu.Application.Cities.Commands.DeleteCity;
using FastDeliveruu.Application.Cities.Commands.UpdateCity;
using FastDeliveruu.Application.Cities.Queries.GetAllCities;
using FastDeliveruu.Application.Cities.Queries.GetCityById;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cities")]
public class CitiesController : ApiController
{
    private readonly IMediator _mediator;

    public CitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<CityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCities([FromQuery] DefaultParams defaultParams)
    {
        GetAllCitiesQuery query = new GetAllCitiesQuery(defaultParams);
        PagedList<CityDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpGet("{id:int}", Name = "GetCityById")]
    [ProducesResponseType(typeof(CityDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCityById(int id)
    {
        GetCityByIdQuery query = new GetCityByIdQuery(id);
        Result<CityDetailDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
    {
        Result<CityDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return CreatedAtRoute(
            nameof(GetCityById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityCommand command)
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

    [HttpDelete("{id:int}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCity(int id)
    {
        DeleteCityCommand command = new DeleteCityCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return NoContent();
    }
}
