using Asp.Versioning;
using FastDeliveruu.Application.Authentication.Commands.ChangePassword;
using FastDeliveruu.Application.Authentication.Commands.ResetPassword;
using FastDeliveruu.Application.Authentication.Commands.UserRegister;
using FastDeliveruu.Application.Authentication.Queries.ConfirmPhoneNumber;
using FastDeliveruu.Application.Authentication.Queries.ForgotPassword;
using FastDeliveruu.Application.Authentication.Queries.SendConfirmPhoneNumber;
using FastDeliveruu.Application.Authentication.Queries.UserEmailConfirm;
using FastDeliveruu.Application.Authentication.Queries.UserLogin;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class UserAuthController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public UserAuthController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] UserRegisterCommand command)
    {
        Result<UserAuthenticationResponse> authenticationResult = await _mediator.Send(command);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        return Ok(authenticationResult.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] UserLoginQuery query)
    {
        Result<UserAuthenticationResponse> authenticationResult = await _mediator.Send(query);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        return Ok(authenticationResult.Value);
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(string email, string encodedToken)
    {
        UserEmailConfirmQuery query = new UserEmailConfirmQuery(email, encodedToken);

        Result<bool> isConfirmEmailResult = await _mediator.Send(query);
        if (isConfirmEmailResult.IsFailed)
        {
            return Problem(isConfirmEmailResult.Errors);
        }

        return Redirect($"{_configuration["AppSettings:RedirectUrl"]}/authen/login");
    }

    [HttpGet("send-confirm-phone-number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendConfirmPhoneNumber(string phoneNumber)
    {
        SendConfirmPhoneNumberQuery query = new SendConfirmPhoneNumberQuery(User.GetCurrentUserId(), phoneNumber);

        Result result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return Ok();
    }

    [HttpGet("confirm-phone-number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPhoneNumber(string otpCode)
    {
        ConfirmPhoneNumberQuery query = new ConfirmPhoneNumberQuery(User.GetCurrentUserId(), otpCode);

        Result result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return Ok();
    }

    [HttpGet("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        ForgotPasswordQuery query = new ForgotPasswordQuery(email);
        Result result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok();
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        command.UserId = User.GetCurrentUserId().ToString();
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok();
    }
}