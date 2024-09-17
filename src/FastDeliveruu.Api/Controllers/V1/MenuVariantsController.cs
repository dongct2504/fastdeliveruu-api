using Asp.Versioning;
using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;
using FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Queries.GetById;
using FastDeliveruu.Application.MenuVariants.Queries.GetByMenuItem;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-variants")]
public class MenuVariantsController : ApiController
{
    private readonly IMediator _mediator;

    public MenuVariantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-by-menuitem/{menuItemId:guid}")]
    [ProducesResponseType(typeof(List<MenuVariantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMenuItem(Guid menuItemId)
    {
        GetByMenuItemQuery query = new GetByMenuItemQuery(menuItemId);
        List<MenuVariantDto> menuVariantDtos = await _mediator.Send(query);
        return Ok(menuVariantDtos);
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    [ProducesResponseType(typeof(MenuVariantDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdQuery query = new GetByIdQuery(id);
        Result<MenuVariantDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MenuVariantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMenuVariant([FromBody] CreateMenuVariantCommand command)
    {
        Result<MenuVariantDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return CreatedAtRoute(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuVariant(Guid id, [FromBody] UpdateMenuVariantCommand command)
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuVariant(Guid id)
    {
        DeleteMenuItemCommand command = new DeleteMenuItemCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }
}
