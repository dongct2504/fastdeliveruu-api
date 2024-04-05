using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
[Route("api/user-auth")]
public class UsersController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly ILocalUserServices _localUserServices;

    public UsersController(ILocalUserServices localUserServices)
    {
        _apiResponse = new ApiResponse();
        _localUserServices = localUserServices;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Login([FromForm] LoginRequestDto loginRequestDto)
    {
        try
        {
            LoginResponseDto loginResponseDto = await _localUserServices.LoginAsync(loginRequestDto);
            if (loginResponseDto.LocalUserDto == null || string.IsNullOrEmpty(loginResponseDto.Token))
            {
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username of password is incorrect.");

                return BadRequest(_apiResponse);
            }

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = loginResponseDto;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Register(
        [FromForm] RegisterationRequestDto registerationRequestDto)
    {
        try
        {
            if (!await _localUserServices.IsUniqueUserAsync(registerationRequestDto.UserName))
            {
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username is already exists.");

                return BadRequest(_apiResponse);
            }

            LocalUserDto localUserDto = await _localUserServices.RegisterAsync(registerationRequestDto);

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }
}