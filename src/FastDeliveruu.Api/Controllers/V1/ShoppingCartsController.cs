using Asp.Versioning;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.ShoppingCarts.Commands.CreateShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.DeleteShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Queries.GetAllShoppingCarts;
using FastDeliveruu.Application.ShoppingCarts.Queries.GetShoppingCartById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shopping-carts")]
public class ShoppingCartsController : ApiController
{
    private readonly IMediator _mediator;

    public ShoppingCartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(PaginationResponse<ShoppingCartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShoppingCartsByUserId(Guid userId, int page = 1)
    {
        GetAllShoppingCartsByUserIdQuery query = new GetAllShoppingCartsByUserIdQuery(userId, page);
        PaginationResponse<ShoppingCartDto> paginationResponse = await _mediator.Send(query);
        return Ok(paginationResponse);
    }

    [HttpGet("{userId:guid}/{menuItemId:guid}", Name = "GetShoppingCartById")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShoppingCartById(Guid userId, Guid menuItemId)
    {
        GetShoppingCartByIdQuery query = new GetShoppingCartByIdQuery(userId, menuItemId);
        Result<ShoppingCartDto> getShoppingCartResult = await _mediator.Send(query);
        if (getShoppingCartResult.IsFailed)
        {
            return Problem(getShoppingCartResult.Errors);
        }

        return Ok(getShoppingCartResult.Value);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddToCart([FromBody] CreateShoppingCartCommand command)
    {
        Result addToCartResult = await _mediator.Send(command);
        if (addToCartResult.IsFailed)
        {
            return Problem(addToCartResult.Errors);
        }

        return Ok();
    }

    [HttpPut("{userId:guid}/{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateShoppingCart(
        Guid userId,
        Guid menuItemId,
        [FromBody] UpdateShoppingCartCommand command)
    {
        if (userId != command.LocalUserId || menuItemId != command.MenuItemId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }
        Result updateShoppingCartResult = await _mediator.Send(command);
        if (updateShoppingCartResult.IsFailed)
        {
            return Problem(updateShoppingCartResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{userId:guid}/{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteShoppingCart(Guid userId, Guid menuItemId)
    {
        DeleteShoppingCartCommand command = new DeleteShoppingCartCommand(userId, menuItemId);
        Result deleteShoppingCartResult = await _mediator.Send(command);
        if (deleteShoppingCartResult.IsFailed)
        {
            return Problem(deleteShoppingCartResult.Errors);
        }

        return NoContent();
    }
}