using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
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
    private readonly ApiResponse _apiResponse;
    private readonly PaginationResponse _paginationResponse;
    private readonly ILocalUserServices _localUserServices;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(ILocalUserServices localUserServices,
        ILogger<UsersController> logger,
        IMapper mapper)
    {
        _apiResponse = new ApiResponse();
        _paginationResponse = new PaginationResponse();
        _localUserServices = localUserServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginationResponse>> GetAllUsers(int page = 1)
    {
        try
        {
            IEnumerable<LocalUser> localUsers = await _localUserServices.GetAllLocalUserAsync(page);

            _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _paginationResponse.IsSuccess = true;

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.UserPageSize;
            _paginationResponse.TotalRecords = await _localUserServices.GetTotalLocalUsersAsync();

            _paginationResponse.Result = _mapper.Map<IEnumerable<LocalUserDto>>(localUsers);

            return Ok(_paginationResponse);
        }
        catch (Exception ex)
        {
            _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _paginationResponse.IsSuccess = false;
            _paginationResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _paginationResponse);
        }
    }
}