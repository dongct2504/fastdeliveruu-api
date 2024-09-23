using Asp.Versioning;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCartItem;
using FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCustomerCart;
using FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;
using FastDeliveruu.Application.ShoppingCarts.Queries.GetCustomerCart;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/carts")]
public class CustomerCartsController : ApiController
{
    private readonly IMediator _mediator;

    public CustomerCartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ShoppingCartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCutomerCart()
    {
        Guid userId = User.GetCurrentUserId();

        GetCustomerCartQuery query = new GetCustomerCartQuery(userId);
        List<ShoppingCartDto> customerCart = await _mediator.Send(query);
        return Ok(customerCart);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();

        Result<int> updatedCartResult = await _mediator.Send(command);
        if (updatedCartResult.IsFailed)
        {
            return Problem(updatedCartResult.Errors);
        }

        return Ok(updatedCartResult.Value);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomerCart()
    {
        Guid userId = User.GetCurrentUserId();

        DeleteCustomerCartCommand command = new DeleteCustomerCartCommand(userId);
        Result deleteShoppingCartResult = await _mediator.Send(command);
        if (deleteShoppingCartResult.IsFailed)
        {
            return Problem(deleteShoppingCartResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        Guid userId = User.GetCurrentUserId();

        DeleteCartItemCommand command = new DeleteCartItemCommand(userId, id);
        Result<int> updatedCartResult = await _mediator.Send(command);
        if (updatedCartResult.IsFailed)
        {
            return Problem(updatedCartResult.Errors);
        }

        return Ok(updatedCartResult.Value);
    }
}