using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Users.Commands.DeleteUser;
using FastDeliveruu.Application.Users.Commands.EditUserRoles;
using FastDeliveruu.Application.Users.Commands.UpdateUser;
using FastDeliveruu.Application.Users.Queries.GetAllUsers;
using FastDeliveruu.Application.Users.Queries.GetAllUsersWithRoles;
using FastDeliveruu.Application.Users.Queries.GetUserById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize(Policy = PolicyConstants.RequiredAdmin)]
[Route("api/v{version:apiVersion}/admin")]
public class AdminController : ApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<AppUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery] AppUserParams appUserParams)
    {
        GetAllUsersQuery query = new GetAllUsersQuery(appUserParams);
        PagedList<AppUserDto> getAllUsers = await _mediator.Send(query);
        return Ok(getAllUsers);
    }

    [HttpGet("get-users-with-roles")]
    [ProducesResponseType(typeof(PagedList<AppUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsersWithRoles([FromQuery] DefaultParams defaultParams)
    {
        GetAllUsersWithRolesQuery query = new GetAllUsersWithRolesQuery(defaultParams);
        PagedList<AppUserWithRolesDto> getAllUsers = await _mediator.Send(query);
        return Ok(getAllUsers);
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

    [HttpPost("edit-user-roles/{id:guid}")]
    [ProducesResponseType(typeof(PagedList<AppUserWithRolesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditUserRoles(Guid id, [FromQuery] string roles)
    {
        EditUserRolesCommand command = new EditUserRolesCommand(id, roles);
        Result<string[]> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
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
