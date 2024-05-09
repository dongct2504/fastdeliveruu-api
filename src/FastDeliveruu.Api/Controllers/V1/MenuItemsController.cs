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
    [ResponseCache(CacheProfileName = CacheProfileConstants.Default30,
        VaryByQueryKeys = new[] { "genreId", "restaurantId", "page" })]
    [ProducesResponseType(typeof(PaginationResponse<MenuItemDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMenuItems(Guid? genreId, Guid? restaurantId, int page = 1)
    {
        try
        {
            GetAllMenuItemsQuery query = new GetAllMenuItemsQuery(genreId, restaurantId, page);
            PaginationResponse<MenuItemDetailDto> paginationResponse = await _mediator.Send(query);
            return Ok(paginationResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id:guid}", Name = "GetMenuItemById")]
    [ProducesResponseType(typeof(MenuItemDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemById(Guid id)
    {
        try
        {
            GetMenuItemByIdQuery query = new GetMenuItemByIdQuery(id);
            Result<MenuItemDetailDto> getMenuItemResult = await _mediator.Send(query);
            if (getMenuItemResult.IsFailed)
            {
                return Problem(getMenuItemResult.Errors);
            }

            return Ok(getMenuItemResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemCommand command)
    {
        try
        {
            Result<MenuItemDto> createMenuItemResult = await _mediator.Send(command);
            if (createMenuItemResult.IsFailed)
            {
                return Problem(createMenuItemResult.Errors);
            }

            return CreatedAtRoute(
                nameof(GetMenuItemById),
                new { Id = createMenuItemResult.Value.MenuItemId },
                createMenuItemResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateMenuItem(Guid id, [FromForm] UpdateMenuItemCommand command)
    {
        try
        {
            if (id != command.MenuItemId)
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
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteMenuItem(Guid id)
    {
        try
        {
            DeleteMenuItemCommand command = new DeleteMenuItemCommand(id);
            Result deleteMenuItemResult = await _mediator.Send(command);
            if (deleteMenuItemResult.IsFailed)
            {
                return Problem(deleteMenuItemResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}