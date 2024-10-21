using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Orders.Commands.DeleteOrder;
using FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;
using FastDeliveruu.Application.Orders.Queries.GetOrderById;
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

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.RequiredAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
