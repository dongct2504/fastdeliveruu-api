using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using Microsoft.AspNetCore.Mvc;
using FluentResults;
using MediatR;
using FastDeliveruu.Application.Users.Queries.GetAllUsers;
using FastDeliveruu.Application.Users.Queries.GetUserById;
using FastDeliveruu.Application.Users.Commands.UpdateUser;
using FastDeliveruu.Application.Users.Commands.DeleteUser;
using Microsoft.AspNetCore.Authorization;
using FastDeliveruu.Application.Common.Constants;
using Asp.Versioning;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(PaginationResponse<LocalUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(int page = 1)
    {
        GetAllUsersQuery query = new GetAllUsersQuery(page);
        PaginationResponse<LocalUserDto> getAllUsers = await _mediator.Send(query);

        return Ok(getAllUsers);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(LocalUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        GetUserByIdQuery query = new GetUserByIdQuery(id);
        Result<LocalUserDto> getUserResult = await _mediator.Send(query);
        if (getUserResult.IsFailed)
        {
            return Problem(getUserResult.Errors);
        }

        return Ok(getUserResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Customer + "," + RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserCommand command)
    {
        if (id != command.LocalUserId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updateLocalUserResult = await _mediator.Send(command);
        if (updateLocalUserResult.IsFailed)
        {
            return Problem(updateLocalUserResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        DeleteUserCommand command = new DeleteUserCommand(id);
        Result deleteUserResult = await _mediator.Send(command);
        if (deleteUserResult.IsFailed)
        {
            return Problem(deleteUserResult.Errors);
        }

        return NoContent();
    }
}