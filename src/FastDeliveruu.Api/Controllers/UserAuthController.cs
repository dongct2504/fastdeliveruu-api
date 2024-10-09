using Asp.Versioning;
using FastDeliveruu.Application.Authentication.Commands.UserRegister;
using FastDeliveruu.Application.Authentication.Queries.UserEmailConfirm;
using FastDeliveruu.Application.Authentication.Queries.UserLogin;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class UserAuthController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public UserAuthController(
        IMediator mediator,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _emailSender = emailSender;
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

        //await SendEmailAsync(authenticationResult.Value.AppUserDto.Email, authenticationResult.Value.Token);

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmail(string email, string encodedToken)
    {
        UserEmailConfirmQuery query = new UserEmailConfirmQuery(email, encodedToken);

        Result<bool> isConfirmEmailResult = await _mediator.Send(query);
        if (isConfirmEmailResult.IsFailed)
        {
            return Problem(isConfirmEmailResult.Errors);
        }

        string redirectUrlBase = _configuration.GetValue<string>("RedirectUrl");

        return Redirect($"{redirectUrlBase}/auth/login");
    }

    [NonAction]
    private async Task SendEmailAsync(string email, string encodedToken)
    {
        string? confirmationLink = Url.Action(
            action: nameof(ConfirmEmail),
            controller: "UserAuth",
            values: new { email, encodedToken },
            Request.Scheme);

        string receiver = email;
        string subject = "Cảm ơn vì đã đăng kí tại FastDeliveruu";
        string messageBody = $"Vui lòng xác nhận email tại đây: {confirmationLink}";

        await _emailSender.SendEmailAsync(receiver, subject, messageBody);
    }
}