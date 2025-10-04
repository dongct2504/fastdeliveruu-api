using Asp.Versioning;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Wards.Commands.CreateWard;
using FastDeliveruu.Application.Wards.Commands.DeleteWard;
using FastDeliveruu.Application.Wards.Commands.UpdateWard;
using FastDeliveruu.Application.Wards.Queries.GetAllWards;
using FastDeliveruu.Application.Wards.Queries.GetWardById;
using FastDeliveruu.Application.Wards.Queries.GetWardsByDistrict;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wards")]
public class WardsController : ApiController
{
    private readonly IMediator _mediator;

    public WardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<WardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWards([FromQuery] DefaultParams defaultParams)
    {
        GetAllWardsQuery query = new GetAllWardsQuery(defaultParams);
        PagedList<WardDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpGet("get-by-district")]
    [ProducesResponseType(typeof(PagedList<WardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWardsByDistrict([FromQuery] WardParams wardParams)
    {
        GetWardsByDistrictQuery query = new GetWardsByDistrictQuery(wardParams);
        Result<PagedList<WardDto>> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpGet("{id:int}", Name = "GetWardById")]
    [ProducesResponseType(typeof(WardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWardById(int id)
    {
        GetWardByIdQuery query = new GetWardByIdQuery(id);
        Result<WardDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(WardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateWard([FromBody] CreateWardCommand command)
    {
        Result<WardDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return CreatedAtRoute(
            nameof(GetWardById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateWard(int id, [FromBody] UpdateWardCommand command)
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
    public async Task<IActionResult> DeleteWard(int id)
    {
        DeleteWardCommand command = new DeleteWardCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return NoContent();
    }
}
