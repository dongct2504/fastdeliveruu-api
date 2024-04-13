using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly IAuthenticationServices _authenticationServices;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationServices authenticationServices,
        ILogger<AuthenticationController> logger)
    {
        _apiResponse = new ApiResponse();
        _authenticationServices = authenticationServices;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Register(
        [FromBody] RegisterationRequestDto registerationRequestDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuthenticationResult authenticationResult = await _authenticationServices.
                RegisterAsync(registerationRequestDto);
            if (authenticationResult.LocalUserDto == null || string.IsNullOrEmpty(authenticationResult.Token))
            {
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Conflict;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username is already exists.");

                return Conflict(_apiResponse);
            }

            AuthenticationResponse registerationResponseDto = new AuthenticationResponse
            {
                LocalUserDto = authenticationResult.LocalUserDto,
                Token = authenticationResult.Token
            };

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = registerationResponseDto;

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

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        try
        {
            AuthenticationResult authenticationResult = await _authenticationServices
                .LoginAsync(loginRequestDto);
            if (authenticationResult.LocalUserDto == null || string.IsNullOrEmpty(authenticationResult.Token))
            {
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username of password is incorrect.");

                return BadRequest(_apiResponse);
            }

            AuthenticationResponse loginResponseDto = new AuthenticationResponse
            {
                LocalUserDto = authenticationResult.LocalUserDto,
                Token = authenticationResult.Token
            };

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
}