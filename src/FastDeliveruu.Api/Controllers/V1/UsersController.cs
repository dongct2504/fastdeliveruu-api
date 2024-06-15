using FastDeliveruu.Application.Dtos;
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
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Application.Dtos.AppUserDtos;

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

    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(PagedList<AppUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 6)
    {
        GetAllUsersQuery query = new GetAllUsersQuery(pageNumber, pageSize);
        PagedList<AppUserDto> getAllUsers = await _mediator.Send(query);

        return Ok(getAllUsers);
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

    [HttpGet("{id:guid}", Name = "GetUserById")]
    [ProducesResponseType(typeof(AppUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        GetUserByIdQuery query = new GetUserByIdQuery(id);
        Result<AppUserDetailDto> getUserResult = await _mediator.Send(query);
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
    public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updatedUserResult = await _mediator.Send(command);
        if (updatedUserResult.IsFailed)
        {
            return Problem(updatedUserResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Customer + "," + RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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