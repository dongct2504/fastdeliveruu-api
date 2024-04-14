using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[Authorize(Roles = RoleConstants.RoleAdmin)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly PaginationResponse<LocalUserDto> _paginationResponse;
    private readonly ILocalUserServices _localUserServices;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(ILocalUserServices localUserServices,
        ILogger<UsersController> logger,
        IMapper mapper)
    {
        _paginationResponse = new PaginationResponse<LocalUserDto>();
        _localUserServices = localUserServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
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
}