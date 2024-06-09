using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Shippers.Commands.DeleteShipper;
using FastDeliveruu.Application.Shippers.Commands.UpdateShipper;
using FastDeliveruu.Application.Shippers.Queries.GetAllShippers;
using FastDeliveruu.Application.Shippers.Queries.GetShipperById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shippers")]
public class ShippersController : ApiController
{
    private readonly IMediator _mediator;

    public ShippersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(PagedList<ShipperDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShippers(int pageNumber = 1, int pageSize = 6)
    {
        GetAllShippersQuery query = new GetAllShippersQuery(pageNumber, pageSize);
        PagedList<ShipperDto> getAllShippers = await _mediator.Send(query);
        return Ok(getAllShippers);
    }

    [HttpGet("{id:guid}", Name = "GetShipperById")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(ShipperDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShipperById(Guid id)
    {
        GetShipperByIdQuery query = new GetShipperByIdQuery(id);
        Result<ShipperDetailDto> getShipperResult = await _mediator.Send(query);
        if (getShipperResult.IsFailed)
        {
            return Problem(getShipperResult.Errors);
        }

        return Ok(getShipperResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShipper(Guid id, [FromForm] UpdateShipperCommand command)
    {
        if (id != command.ShipperId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updateShipperResult = await _mediator.Send(command);
        if (updateShipperResult.IsFailed)
        {
            return Problem(updateShipperResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShipper(Guid id)
    {
        DeleteShipperCommand command = new DeleteShipperCommand(id);
        Result deleteShipperResult = await _mediator.Send(command);
        if (deleteShipperResult.IsFailed)
        {
            return Problem(deleteShipperResult.Errors);
        }

        return NoContent();
    }
}