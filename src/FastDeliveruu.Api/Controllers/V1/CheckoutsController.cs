using Asp.Versioning;
using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Orders.Commands.CreateOrder;
using FastDeliveruu.Application.Orders.Commands.UpdateVnpay;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/checkouts")]
public class CheckoutsController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly IVnpayServices _vnPayServices;

    public CheckoutsController(
        IMediator mediator,
        IConfiguration configuration,
        IVnpayServices vnPayServices)
    {
        _mediator = mediator;
        _configuration = configuration;
        _vnPayServices = vnPayServices;
    }

    [HttpPost("checkout-cash")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckoutCash([FromBody] CreateOrderCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();

        Result<Order> createOrderResult = await _mediator.Send(command);
        if (createOrderResult.IsFailed)
        {
            return Problem(createOrderResult.Errors);
        }

        PaymentResponse paymentResponse = new PaymentResponse()
        {
            IsSuccess = true,
            OrderId = createOrderResult.Value.Id,
            OrderDescription = createOrderResult.Value.OrderDescription ?? string.Empty,
            PaymentMethod = (PaymentMethodsEnum)(createOrderResult.Value.PaymentMethod ?? 0),
            TotalAmount = createOrderResult.Value.TotalAmount,
            TransactionId = createOrderResult.Value.TransactionId ?? "0"
        };

        return Ok(paymentResponse);
    }

    [HttpPost("checkout-vnpay")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckoutVnpay([FromBody] CreateOrderCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();

        Result<Order> createOrderResult = await _mediator.Send(command);
        if (createOrderResult.IsFailed)
        {
            return Problem(createOrderResult.Errors);
        }

        PaymentResponse paymentResponse = new PaymentResponse()
        {
            IsSuccess = true,
            OrderId = createOrderResult.Value.Id,
            VnpayReturnUrl = _vnPayServices.CreatePaymentUrl(HttpContext, createOrderResult.Value)
        };

        return Ok(paymentResponse);
    }

    [AllowAnonymous]
    [HttpGet("vnpay-return")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VnpayReturn()
    {
        PaymentResponse? vnpayResponse = _vnPayServices.PaymentExecute(Request.Query);
        if (vnpayResponse == null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "can't create the response.");
        }

        UpdateVnpayCommand command = new UpdateVnpayCommand(vnpayResponse);
        Result<PaymentResponse> updateVnpayResult = await _mediator.Send(command);
        if (updateVnpayResult.IsFailed)
        {
            return Problem(updateVnpayResult.Errors);
        }

        string redirectUrlBase = _configuration.GetValue<string>("RedirectUrl");

        if (updateVnpayResult.Value.IsSuccess)
        {
            return Redirect(Utils
                .CreateResponsePaymentUrl($"{redirectUrlBase}/checkout/success", updateVnpayResult.Value));
        }
        else
        {
            return Redirect(Utils
                .CreateResponsePaymentUrl($"{redirectUrlBase}/checkout/failed", updateVnpayResult.Value));
        }
    }
}
