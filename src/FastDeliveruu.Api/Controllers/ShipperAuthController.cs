using Asp.Versioning;
using FastDeliveruu.Application.Authentication.Commands.ShipperRegister;
using FastDeliveruu.Application.Authentication.Queries.ShipperEmailConfirm;
using FastDeliveruu.Application.Authentication.Queries.ShipperLogin;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/shipper-auth")]
public class ShipperAuthController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public ShipperAuthController(
        IMediator mediator,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ShipperAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] ShipperRegisterCommand command)
    {
        Result<ShipperAuthenticationResponse> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        await SendEmailAsync(result.Value.ShipperDto.Email, result.Value.Token);

        return Ok(result.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ShipperAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] ShipperLoginQuery query)
    {
        Result<ShipperAuthenticationResponse> authenticationResult = await _mediator.Send(query);
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
        ShipperEmailConfirmQuery query = new ShipperEmailConfirmQuery(email, encodedToken);
        Result result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        string redirectUrlBase = _configuration.GetValue<string>("RedirectUrl");
        return Redirect($"{redirectUrlBase}/shipper-auth/login");
    }

    [NonAction]
    private async Task SendEmailAsync(string email, string encodedToken)
    {
        string? confirmationLink = Url.Action(
            action: nameof(ConfirmEmail),
            controller: "ShipperAuth",
            values: new { email, encodedToken },
            Request.Scheme);

        string receiver = email;
        string subject = "Cảm ơn vì đã đăng kí tại FastDeliveruu";
        string messageBody = $"Vui lòng xác nhận email tại đây: {confirmationLink}";

        await _emailSender.SendEmailAsync(receiver, subject, messageBody);
    }
}
