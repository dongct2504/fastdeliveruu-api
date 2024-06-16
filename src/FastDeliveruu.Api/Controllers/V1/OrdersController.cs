using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Orders.Commands.CreateOrder;
using FastDeliveruu.Application.Orders.Commands.DeleteOrder;
using FastDeliveruu.Application.Orders.Commands.UpdateVnpay;
using FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;
using FastDeliveruu.Application.Orders.Queries.GetDeliveryMethods;
using FastDeliveruu.Application.Orders.Queries.GetOrderById;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IVnpayServices _vnPayServices;

    public OrdersController(IMediator mediator, IVnpayServices vnPayServices)
    {
        _mediator = mediator;
        _vnPayServices = vnPayServices;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 6)
    {
        Guid userId = User.GetCurrentUserId();

        GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery(userId, pageNumber, pageSize);
        PagedList<OrderDto> paginationResponse = await _mediator.Send(query);
        return Ok(paginationResponse);
    }

    [HttpGet("{orderId:guid}", Name = "GetOrderbyId")]
    [ProducesResponseType(typeof(OrderHeaderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderbyId(Guid orderId)
    {
        Guid userId = User.GetCurrentUserId();

        GetOrderByIdQuery query = new GetOrderByIdQuery(userId, orderId);
        Result<OrderHeaderDetailDto> getOrderResult = await _mediator.Send(query);
        if (getOrderResult.IsFailed)
        {
            return Problem(getOrderResult.Errors);
        }

        return Ok(getOrderResult.Value);
    }

    [HttpGet("delivery-methods")]
    [ProducesResponseType(typeof(List<DeliveryMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeliveryMethods()
    {
        GetDeliveryMethodsQuery query = new GetDeliveryMethodsQuery();
        List<DeliveryMethodDto> deliveryMethodDtos = await _mediator.Send(query);
        return Ok(deliveryMethodDtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderCommand command)
    {
        command.AppUserId = User.GetCurrentUserId();

        Result<Order> createOrderResult = await _mediator.Send(command);
        if (createOrderResult.IsFailed)
        {
            return Problem(createOrderResult.Errors);
        }

        string paymentUrl = string.Empty;

        switch (command.PaymentMethod)
        {
            case PaymentMethods.Cash:
                PaymentResponse paymentResponse = new PaymentResponse()
                {
                    IsSuccess = true,
                    OrderId = createOrderResult.Value.OrderId,
                    OrderDescription = createOrderResult.Value.OrderDescription ?? string.Empty,
                    PaymentMethod = createOrderResult.Value.PaymentMethod ?? PaymentMethods.Cash,
                    TotalAmount = createOrderResult.Value.TotalAmount,
                    TransactionId = createOrderResult.Value.TransactionId ?? "0"
                };
                return Ok(paymentResponse);
            case PaymentMethods.Vnpay:
                paymentUrl = _vnPayServices.CreatePaymentUrl(HttpContext, createOrderResult.Value);
                break;
        }

        return Ok(paymentUrl);
    }

    [AllowAnonymous]
    [HttpGet("vnpay-return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VnpayReturn()
    {
        VnpayResponse? vnpayResponse = _vnPayServices.PaymentExecute(Request.Query);
        if (vnpayResponse == null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "can't create the response.");
        }

        UpdateVnpayCommand command = new UpdateVnpayCommand(vnpayResponse);
        Result<VnpayResponse> updateVnpayResult = await _mediator.Send(command);
        if (updateVnpayResult.IsFailed)
        {
            return Problem(updateVnpayResult.Errors);
        }

        return Ok(updateVnpayResult.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        DeleteOrderCommand command = new DeleteOrderCommand(id);
        Result deleteOrderResult = await _mediator.Send(command);
        if (deleteOrderResult.IsFailed)
        {
            return Problem(deleteOrderResult.Errors);
        }

        return NoContent();
    }
}
