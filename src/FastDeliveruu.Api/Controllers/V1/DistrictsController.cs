using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Districts.Commands.CreateDistrict;
using FastDeliveruu.Application.Districts.Commands.DeleteDistrict;
using FastDeliveruu.Application.Districts.Commands.UpdateDistrict;
using FastDeliveruu.Application.Districts.Queries.GetAllDistricts;
using FastDeliveruu.Application.Districts.Queries.GetDistrictById;
using FastDeliveruu.Application.Districts.Queries.GetDistrictsByCity;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/districts")]
public class DistrictsController : ApiController
{
    private readonly IMediator _mediator;

    public DistrictsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<DistrictDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDistricts([FromQuery] DefaultParams defaultParams)
    {
        GetAllDistrictsQuery query = new GetAllDistrictsQuery(defaultParams);
        PagedList<DistrictDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpGet("get-by-city")]
    [ProducesResponseType(typeof(PagedList<DistrictDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDistrictsByCity([FromQuery] DistrictParams districtParams)
    {
        GetDistrictsByCityQuery query = new GetDistrictsByCityQuery(districtParams);
        Result<PagedList<DistrictDto>> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpGet("{id:int}", Name = "GetDistrictById")]
    [ProducesResponseType(typeof(DistrictDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDistrictById(int id)
    {
        GetDistrictByIdQuery query = new GetDistrictByIdQuery(id);
        Result<DistrictDetailDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(DistrictDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDistrict([FromBody] CreateDistrictCommand command)
    {
        Result<DistrictDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return CreatedAtRoute(
            nameof(GetDistrictById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDistrict(int id, [FromBody] UpdateDistrictCommand command)
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
    public async Task<IActionResult> DeleteDistrict(int id)
    {
        DeleteDistrictCommand command = new DeleteDistrictCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return NoContent();
    }
}
