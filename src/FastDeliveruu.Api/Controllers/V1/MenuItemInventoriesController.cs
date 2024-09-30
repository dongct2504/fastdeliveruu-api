using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItemInventories.Commands.CreateMenuItemInventory;
using FastDeliveruu.Application.MenuItemInventories.Commands.DeleteMenuItemInventory;
using FastDeliveruu.Application.MenuItemInventories.Queries.GetAllMenuItemInventories;
using FastDeliveruu.Application.MenuItemInventories.Queries.GetMenuItemInventoryById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Route("api/v{version:apiVersion}/menu-item-inventories")]
public class MenuItemInventoriesController : ApiController
{
    private readonly IMediator _mediator;

    public MenuItemInventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<MenuItemInventoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMenuItemInventories([FromQuery] DefaultParams defaultParams)
    {
        GetAllMenuItemInventoriesQuery query = new GetAllMenuItemInventoriesQuery(defaultParams);
        PagedList<MenuItemInventoryDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpGet("{id:guid}", Name = "GetMenuItemInventoryById")]
    [ProducesResponseType(typeof(MenuItemInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemInventoryById(Guid id)
    {
        GetMenuItemInventoryByIdQuery query = new GetMenuItemInventoryByIdQuery(id);
        Result<MenuItemInventoryDto> result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MenuItemInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItemInventory([FromBody] UpdateMenuItemInventoryCommand command)
    {
        Result<MenuItemInventoryDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItemInventory(Guid id)
    {
        DeleteMenuItemInventoryCommand command = new DeleteMenuItemInventoryCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return NoContent();
    }
}
