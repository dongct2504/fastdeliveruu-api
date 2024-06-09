using Asp.Versioning;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.ShoppingCarts.Commands.CreateShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.DeleteAllShoppingCarts;
using FastDeliveruu.Application.ShoppingCarts.Commands.DeleteShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;
using FastDeliveruu.Application.ShoppingCarts.Queries.GetAllShoppingCarts;
using FastDeliveruu.Application.ShoppingCarts.Queries.GetShoppingCartById;
using FastDeliveruu.Domain.Extensions;
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

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<ShoppingCartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShoppingCarts(int pageNumber = 1, int pageSize = 6)
    {
        Guid userId = User.GetCurrentUserId();

        GetAllShoppingCartsByUserIdQuery query = new GetAllShoppingCartsByUserIdQuery(userId, pageNumber, pageSize);
        PagedList<ShoppingCartDto> paginationResponse = await _mediator.Send(query);
        return Ok(paginationResponse);
    }

    [HttpGet("{menuItemId:guid}", Name = "GetShoppingCartById")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShoppingCartById(Guid menuItemId)
    {
        Guid userId = User.GetCurrentUserId();

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
    public async Task<IActionResult> AddToCart([FromBody] CreateShoppingCartCommand command)
    {
        command.LocalUserId = User.GetCurrentUserId();

        Result addToCartResult = await _mediator.Send(command);
        if (addToCartResult.IsFailed)
        {
            return Problem(addToCartResult.Errors);
        }

        return Ok();
    }

    [HttpPut("{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShoppingCart(
        Guid menuItemId,
        [FromBody] UpdateShoppingCartCommand command)
    {
        if (menuItemId != command.MenuItemId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        command.LocalUserId = User.GetCurrentUserId();

        Result updateShoppingCartResult = await _mediator.Send(command);
        if (updateShoppingCartResult.IsFailed)
        {
            return Problem(updateShoppingCartResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAllShoppingCarts()
    {
        Guid userId = User.GetCurrentUserId();

        DeleteAllShoppingCartsCommand command = new DeleteAllShoppingCartsCommand(userId);
        Result deleteAllShoppingCartsResult = await _mediator.Send(command);
        if (deleteAllShoppingCartsResult.IsFailed)
        {
            return Problem(deleteAllShoppingCartsResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShoppingCart(Guid menuItemId)
    {
        Guid userId = User.GetCurrentUserId();

        DeleteShoppingCartCommand command = new DeleteShoppingCartCommand(userId, menuItemId);
        Result deleteShoppingCartResult = await _mediator.Send(command);
        if (deleteShoppingCartResult.IsFailed)
        {
            return Problem(deleteShoppingCartResult.Errors);
        }

        return NoContent();
    }
}