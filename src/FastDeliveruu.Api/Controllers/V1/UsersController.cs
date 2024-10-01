using Microsoft.AspNetCore.Mvc;
using FluentResults;
using MediatR;
using FastDeliveruu.Application.Users.Queries.GetUserById;
using FastDeliveruu.Application.Users.Commands.UpdateUser;
using FastDeliveruu.Application.Users.Commands.DeleteUser;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Common.Constants;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("current-user")]
    [ProducesResponseType(typeof(AppUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        Guid userId = User.GetCurrentUserId();

        GetUserByIdQuery query = new GetUserByIdQuery(userId);
        Result<AppUserDetailDto> getUserResult = await _mediator.Send(query);
        if (getUserResult.IsFailed)
        {
            return Problem(getUserResult.Errors);
        }
        return Ok(getUserResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConstants.RequiredCustomerShipper)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCurrentUser(Guid id, [FromForm] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        if (id != User.GetCurrentUserId())
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Not allow!");
        }

        Result updatedUserResult = await _mediator.Send(command);
        if (updatedUserResult.IsFailed)
        {
            return Problem(updatedUserResult.Errors);
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.RequiredCustomerShipper)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCurrentUser(Guid id)
    {
        if (id != User.GetCurrentUserId())
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Not allow!");
        }

        DeleteUserCommand command = new DeleteUserCommand(id);
        Result deleteUserResult = await _mediator.Send(command);
        if (deleteUserResult.IsFailed)
        {
            return Problem(deleteUserResult.Errors);
        }
        return NoContent();
    }
}