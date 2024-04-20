using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ApiController
{
    private readonly PaginationResponse<LocalUserDto> _paginationResponse;
    private readonly ILocalUserServices _localUserServices;
    private readonly IImageServices _imageServices;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(ILocalUserServices localUserServices,
        ILogger<UsersController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _paginationResponse = new PaginationResponse<LocalUserDto>();
        _localUserServices = localUserServices;
        _logger = logger;
        _mapper = mapper;
        _imageServices = imageServices;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers(int page = 1)
    {
        try
        {
            IEnumerable<LocalUser> localUsers = await _localUserServices.GetAllLocalUserAsync(page);

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.UserPageSize;
            _paginationResponse.TotalRecords = await _localUserServices.GetTotalLocalUsersAsync();

            _paginationResponse.Values = _mapper.Map<IEnumerable<LocalUserDto>>(localUsers);

            return Ok(_paginationResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetUserById")]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            Result<LocalUser> getUserResult = await _localUserServices.GetLocalUserByIdAsync(id);
            if (getUserResult.IsFailed)
            {
                return Problem(getUserResult.Errors);
            }

            return Ok(_mapper.Map<LocalUserDto>(getUserResult.Value));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromForm] LocalUserUpdateDto localUserUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<LocalUser> oldLocalUserResult = await _localUserServices.GetLocalUserByIdAsync(id);
            if (oldLocalUserResult.IsFailed)
            {
                return Problem(oldLocalUserResult.Errors);
            }

            LocalUser localUser = oldLocalUserResult.Value;
            string? oldImagePath = oldLocalUserResult.Value.ImageUrl;

            _mapper.Map(localUserUpdateDto, localUser);

            if (localUserUpdateDto.ImageFile != null)
            {
                string uploadImagePath = @"images\users";
                string? fileNameWithExtension = await _imageServices.UploadImageAsync(
                    localUserUpdateDto.ImageFile, uploadImagePath);
                localUser.ImageUrl = @"\images\users\" + fileNameWithExtension;
            }
            localUser.UpdatedAt = DateTime.Now;

            Result updateUserResult = await _localUserServices.UpdateUserAsync(id, localUser);
            if (updateUserResult.IsFailed)
            {
                if (localUserUpdateDto.ImageFile != null)
                {
                    await _imageServices.DeleteImageAsync(localUser.ImageUrl);
                }

                return Problem(updateUserResult.Errors);
            }

            if (localUserUpdateDto.ImageFile != null)
            {
                await _imageServices.DeleteImageAsync(oldImagePath);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            Result<LocalUser> getUserResult = await _localUserServices.GetLocalUserByIdAsync(id);
            if (getUserResult.IsFailed)
            {
                return Problem(getUserResult.Errors);
            }

            await _localUserServices.DeleteUserAsync(id);

            await _imageServices.DeleteImageAsync(getUserResult.Value.ImageUrl);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}