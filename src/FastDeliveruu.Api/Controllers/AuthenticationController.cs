using Asp.Versioning;
using FastDeliveruu.Application.Authentication.Commands.Register;
using FastDeliveruu.Application.Authentication.Commands.RegisterShipper;
using FastDeliveruu.Application.Authentication.Queries.EmailConfirm;
using FastDeliveruu.Application.Authentication.Queries.Login;
using FastDeliveruu.Application.Authentication.Queries.LoginShipper;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/user-auth")]
public class AuthenticationController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IEmailSender _emailSender;

    public AuthenticationController(IMediator mediator, IEmailSender emailSender)
    {
        _mediator = mediator;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        Result<AuthenticationResponse> authenticationResult = await _mediator.Send(command);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        await SendEmailAsync(authenticationResult.Value.LocalUserDto.Email,
            authenticationResult.Value.Token);

        return Ok(authenticationResult.Value);
    }

    [HttpPost("register-shipper")]
    [ProducesResponseType(typeof(AuthenticationShipperResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterShipper([FromBody] RegisterShipperCommand command)
    {
        Result<AuthenticationShipperResponse> authenticationResult = await _mediator.Send(command);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        await SendEmailAsync(authenticationResult.Value.ShipperDto.Email,
            authenticationResult.Value.Token);

        return Ok(authenticationResult.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        Result<AuthenticationResponse> authenticationResult = await _mediator.Send(query);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        return Ok(authenticationResult.Value);
    }

    [HttpPost("login-shipper")]
    [ProducesResponseType(typeof(AuthenticationShipperResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginShipper([FromBody] LoginShipperQuery query)
    {
        Result<AuthenticationShipperResponse> authenticationResult = await _mediator.Send(query);
        if (authenticationResult.IsFailed)
        {
            return Problem(authenticationResult.Errors);
        }

        return Ok(authenticationResult.Value);
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        EmailConfirmQuery query = new EmailConfirmQuery(email, token);

        Result<bool> isConfirmEmailResult = await _mediator.Send(query);
        if (isConfirmEmailResult.IsFailed)
        {
            return Problem(isConfirmEmailResult.Errors);
        }

        return Ok();
    }

    [NonAction]
    private async Task SendEmailAsync(string email, string token)
    {
        string? confirmationLink = Url.Action(
            action: nameof(ConfirmEmail),
            controller: "Authentication",
            values: new { email, token },
            Request.Scheme);

        string receiver = email;
        string subject = "Cảm ơn vì đã đăng kí tại FastDeliveruu";
        string messageBody = $"Vui lòng xác nhận email tại đây: {confirmationLink}";

        await _emailSender.SendEmailAsync(receiver, subject, messageBody);
    }
}