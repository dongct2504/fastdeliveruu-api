using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationServices _authenticationServices;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationServices authenticationServices,
        ILogger<AuthenticationController> logger,
        IEmailSender emailSender)
    {
        _authenticationServices = authenticationServices;
        _logger = logger;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
                return Problem(authenticationResult.Errors);
            }

            string? confirmationLink = Url.Action(
                action: nameof(ConfirmEmail),
                controller: "Authentication",
                values: new
                {
                    token = authenticationResult.Value.Token,
                    email = authenticationResult.Value.LocalUserDto.Email
                },
                // protocol: "https",
                // host: "16f6-116-110-43-52.ngrok-free.app",
                Request.Scheme);

            string receiver = authenticationResult.Value.LocalUserDto.Email;
            string subject = "Cảm ơn vì đã đăng kí tại FastDeliveruu";
            string messageBody = $"Vui lòng xác nhận email tại đây: {confirmationLink}";

            await _emailSender.SendEmailAsync(receiver, subject, messageBody);

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
                return Problem(authenticationResult.Errors);
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

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        try
        {
            Result<bool> isConfirmEmailResult = await _authenticationServices.IsEmailConfirm(token, email);
            if (isConfirmEmailResult.IsFailed)
            {
                return Problem(isConfirmEmailResult.Errors);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}