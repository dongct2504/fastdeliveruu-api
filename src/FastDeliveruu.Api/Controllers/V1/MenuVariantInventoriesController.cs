using Asp.Versioning;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuVariantInventories.Commands.DeleteMenuVariantInventory;
using FastDeliveruu.Application.MenuVariantInventories.Commands.UpdateMenuVariantInventory;
using FastDeliveruu.Application.MenuVariantInventories.Queries.GetAllMenuVariantInventories;
using FastDeliveruu.Application.MenuVariantInventories.Queries.GetMenuVariantInventory;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-variant-inventories")]
public class MenuVariantInventoriesController : ApiController
{
    private readonly IMediator _mediator;

    public MenuVariantInventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<MenuVariantInventoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMenuVariantInventories([FromQuery] DefaultParams defaultParams)
    {
        GetAllMenuVariantInventoriesQuery query = new GetAllMenuVariantInventoriesQuery(defaultParams);
        PagedList<MenuVariantInventoryDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpGet("{id:guid}", Name = "GetMenuVariantInventoryById")]
    [ProducesResponseType(typeof(MenuVariantInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuVariantInventoryById(Guid id)
    {
        GetMenuVariantInventoryQuery query = new GetMenuVariantInventoryQuery(id);
        Result<MenuVariantInventoryDto> result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MenuVariantInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuVariantInventory([FromBody] UpdateMenuVariantInventoryCommand command)
    {
        Result<MenuVariantInventoryDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuVariantInventory(Guid id)
    {
        DeleteMenuVariantInventoryCommand command = new DeleteMenuVariantInventoryCommand(id);
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return NoContent();
    }
}
