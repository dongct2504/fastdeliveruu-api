using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;
using FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;
using FastDeliveruu.Application.MenuItems.Queries.GetAllMenuItems;
using FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-items")]
public class MenuItemsController : ApiController
{
    private readonly IMediator _mediator;

    public MenuItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<MenuItemDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMenuItems([FromQuery] MenuItemParams menuItemParams)
    {
        GetAllMenuItemsQuery query = new GetAllMenuItemsQuery(menuItemParams);
        PagedList<MenuItemDto> paginationResponse = await _mediator.Send(query);
        return Ok(paginationResponse);
    }

    [HttpGet("{id:guid}", Name = "GetMenuItemById")]
    [ProducesResponseType(typeof(MenuItemDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemById(Guid id)
    {
        GetMenuItemByIdQuery query = new GetMenuItemByIdQuery(id);
        Result<MenuItemDetailDto> getMenuItemResult = await _mediator.Send(query);
        if (getMenuItemResult.IsFailed)
        {
            return Problem(getMenuItemResult.Errors);
        }

        return Ok(getMenuItemResult.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemCommand command)
    {
        Result<MenuItemDto> createMenuItemResult = await _mediator.Send(command);
        if (createMenuItemResult.IsFailed)
        {
            return Problem(createMenuItemResult.Errors);
        }

        return CreatedAtRoute(
            nameof(GetMenuItemById),
            new { id = createMenuItemResult.Value.Id },
            createMenuItemResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuItem(Guid id, [FromForm] UpdateMenuItemCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updateMenuItemResult = await _mediator.Send(command);
        if (updateMenuItemResult.IsFailed)
        {
            return Problem(updateMenuItemResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMenuItem(Guid id)
    {
        DeleteMenuItemCommand command = new DeleteMenuItemCommand(id);
        Result deleteMenuItemResult = await _mediator.Send(command);
        if (deleteMenuItemResult.IsFailed)
        {
            return Problem(deleteMenuItemResult.Errors);
        }

        return NoContent();
    }
}