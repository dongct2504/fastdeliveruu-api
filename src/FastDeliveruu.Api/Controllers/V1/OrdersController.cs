using System.Security.Claims;
using FastDeliveruu.Application.Common.Status;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Dtos.VnPayResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ApiController
{
    private readonly IOrderServices _orderServices;
    private readonly IShoppingCartServices _shoppingCartServices;
    private readonly IShipperService _shipperService;
    private readonly IVnPayServices _vnPayServices;
    private readonly ILogger<OrdersController> _logger;
    private readonly IMapper _mapper;

    public OrdersController(IOrderServices orderServices,
        ILogger<OrdersController> logger,
        IMapper mapper,
        IVnPayServices vnPayServices,
        IShoppingCartServices shoppingCartServices,
        IShipperService shipperService)
    {
        _orderServices = orderServices;
        _logger = logger;
        _mapper = mapper;
        _vnPayServices = vnPayServices;
        _shoppingCartServices = shoppingCartServices;
        _shipperService = shipperService;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOrdersByUserId()
    {
        try
        {
            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(
                await _orderServices.GetAllOrdersByUserIdAsynd(userId)));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetOrderbyId")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderbyId(long id)
    {
        try
        {
            Result<Order> getOrderResult = await _orderServices.GetOrderByIdAsync(id);
            if (getOrderResult.IsFailed)
            {
                return Problem(getOrderResult.Errors);
            }

            return Ok(getOrderResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateVnPayUrl([FromForm] OrderCreateDto orderCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            Order order = _mapper.Map<Order>(orderCreateDto);
            order.OrderDate = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;
            order.PaymentMethod = "vnpay";

            IEnumerable<ShoppingCart> shoppingCarts =
                await _shoppingCartServices.GetAllShoppingCartsByUserIdAsync(userId);

            Result<Shipper> getNearestShipperResult = await _shipperService.GetNearestShipperAsync(
                order.Address, order.Ward, order.District, order.City);
            if (getNearestShipperResult.IsFailed)
            {
                return Problem(getNearestShipperResult.Errors);
            }

            order.ShipperId = getNearestShipperResult.Value.ShipperId;
            order.LocalUserId = userId;
            order.OrderDetails = shoppingCarts.Select(cart => new OrderDetail
            {
                OrderId = order.OrderId,
                MenuItemId = cart.MenuItemId,
                Price = cart.MenuItem.Price,
                Quantity = cart.Quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;

            Result<long> createOrderResult = await _orderServices.CreateOrderAsync(order);
            if (createOrderResult.IsFailed)
            {
                return Problem(createOrderResult.Errors);
            }

            string paymentUrl = _vnPayServices.CreatePaymentUrl(HttpContext, order);

            return Ok(paymentUrl);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("vnpay-return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VnPayReturn()
    {
        try
        {
            VnPayResponse? vnPayResponse = _vnPayServices.PaymentExecute(Request.Query);
            if (vnPayResponse == null)
            {
                return BadRequest();
            }

            Result<Order> getOrderResult = await _orderServices.GetOrderByIdAsync(vnPayResponse.OrderId);
            if (getOrderResult.IsFailed)
            {
                return Problem(getOrderResult.Errors);
            }

            Order order = getOrderResult.Value;
            order.OrderDescription = vnPayResponse.OrderDescription;
            order.TransactionId = vnPayResponse.TransactionId;

            if (vnPayResponse.VnPayResponseCode != "00")
            {
                vnPayResponse.IsSuccess = false;
                order.OrderStatus = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Cancelled;
                await _orderServices.UpdateOrderAsync(order.OrderId, order);

                return BadRequest(vnPayResponse);
            }

            vnPayResponse.IsSuccess = true;
            order.OrderStatus = OrderStatus.Success;
            order.PaymentStatus = OrderStatus.Success;
            await _orderServices.UpdateOrderAsync(order.OrderId, order);

            return Ok(vnPayResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [NonAction]
    private Guid GetAuthenticationUserId()
    {
        ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
        Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return Guid.Empty;
        }

        return Guid.Parse(claim.Value);
    }
}
