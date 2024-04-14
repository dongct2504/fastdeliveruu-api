using AutoMapper;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationServices _authenticationServices;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationServices authenticationServices,
        ILogger<AuthenticationController> logger)
    {
        _authenticationServices = authenticationServices;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDto registerationRequestDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<AuthenticationResult> authenticationResult = await _authenticationServices.
                RegisterAsync(registerationRequestDto);
            if (authenticationResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    detail: authenticationResult.Errors[0].Message);
            }

            AuthenticationResponse registerationResponseDto = new AuthenticationResponse
            {
                LocalUserDto = authenticationResult.Value.LocalUserDto,
                Token = authenticationResult.Value.Token
            };

            return Ok(registerationResponseDto);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        try
        {
            Result<AuthenticationResult> authenticationResult = await _authenticationServices
                .LoginAsync(loginRequestDto);
            if (authenticationResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: authenticationResult.Errors[0].Message);
            }

            AuthenticationResponse loginResponseDto = new AuthenticationResponse
            {
                LocalUserDto = authenticationResult.Value.LocalUserDto,
                Token = authenticationResult.Value.Token
            };

            return Ok(loginResponseDto);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}