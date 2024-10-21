using Asp.Versioning;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.WishListDtos;
using FastDeliveruu.Application.WishLists.Commands.DeleteWishList;
using FastDeliveruu.Application.WishLists.Commands.DeleteWishListItem;
using FastDeliveruu.Application.WishLists.Commands.UpdateWishListItem;
using FastDeliveruu.Application.WishLists.Queries.GetWishList;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wishlists")]
public class WishListsController : ApiController
{
    private readonly IMediator _mediator;

    public WishListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<WishListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWishLists([FromQuery] WishListParams wishListParams)
    {
        wishListParams.UserId = User.GetCurrentUserId();
        GetWishListQuery query = new GetWishListQuery(wishListParams);
        PagedList<WishListDto> pagedList = await _mediator.Send(query);
        return Ok(pagedList);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateWishListItem([FromBody] UpdateWishListItemCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();

        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return Ok();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWishList()
    {
        DeleteWishListCommand command = new DeleteWishListCommand(User.GetCurrentUserId());
        Result deleteShoppingCartResult = await _mediator.Send(command);
        if (deleteShoppingCartResult.IsFailed)
        {
            return Problem(deleteShoppingCartResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteWishListItem(Guid id)
    {
        DeleteWishListItemCommand command = new DeleteWishListItemCommand(User.GetCurrentUserId(), id);
        Result updatedCartResult = await _mediator.Send(command);
        if (updatedCartResult.IsFailed)
        {
            return Problem(updatedCartResult.Errors);
        }

        return NoContent();
    }
}
