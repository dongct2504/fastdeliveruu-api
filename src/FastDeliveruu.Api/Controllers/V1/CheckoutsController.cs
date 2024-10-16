using Asp.Versioning;
using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Orders.Commands.CreateOrder;
using FastDeliveruu.Application.Orders.Commands.UpdatePaypal;
using FastDeliveruu.Application.Orders.Commands.UpdateVnpay;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Infrastructure.Helpers;
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
    private readonly PaypalClient _paypalClient;

    public CheckoutsController(
        IMediator mediator,
        IConfiguration configuration,
        IVnpayServices vnPayServices,
        PaypalClient paypalClient)
    {
        _mediator = mediator;
        _configuration = configuration;
        _vnPayServices = vnPayServices;
        _paypalClient = paypalClient;
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

    [HttpPost("create-paypal-order")]
    public async Task<IActionResult> CreatePaypalOrder([FromBody] CreateOrderCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();
        Result<Order> createOrderResult = await _mediator.Send(command);
        if (createOrderResult.IsFailed)
        {
            return Problem(createOrderResult.Errors);
        }

        CreateOrderResponse createOrderResponse = await _paypalClient
            .CreateOrder(command.Amount, command.Currency, command.Reference);

        if (createOrderResponse != null)
        {
            return Ok(new
            {
                OrderId = createOrderResult.Value.Id,
                PaymentOrderId = createOrderResponse.id,
                ApprovalLink = createOrderResponse.links.FirstOrDefault(l => l.rel == "approve")?.href
            });
        }

        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Failed to create PayPal order.");
    }

    [AllowAnonymous]
    [HttpGet("capture-payment-order")]
    public async Task<IActionResult> CapturePaypalOrder(string token, string PayerId)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(PayerId))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Token or PayerId is null");
        }

        CaptureOrderResponse captureOrderResponse = await _paypalClient.CaptureOrder(token);

        if (captureOrderResponse != null)
        {
            UpdatePaypalCommand command = new UpdatePaypalCommand(token, PayerId);
            Result<PaymentResponse> result = await _mediator.Send(command);
            if (result.IsFailed)
            {
                return Problem(result.Errors);
            }

            string redirectUrlBase = _configuration.GetValue<string>("RedirectUrl");

            return Redirect(Utils
                .CreateResponsePaymentUrl($"{redirectUrlBase}/checkout/success", result.Value));
        }

        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Failed to capture PayPal order.");
    }
}
