using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
{
    private readonly ICacheService _cacheService;
    private readonly IOrderRepository _orderRepository;
    private readonly IShipperRepository _shipperRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IShipperRepository shipperRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _shipperRepository = shipperRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Order order = _mapper.Map<Order>(request);
        order.OrderId = Guid.NewGuid();
        order.OrderDescription = $"Create payment for order: {order.OrderId}";
        order.OrderDate = DateTime.Now;
        order.TransactionId = "0";
        order.OrderStatus = OrderStatus.Pending;
        order.PaymentStatus = PaymentStatus.Pending;

        string key = $"{CacheConstants.CustomerCart}-{request.LocalUserId}";

        List<ShoppingCart>? customerCart = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);
        if (customerCart == null || !customerCart.Any())
        {
            string message = "The customer's cart is empty.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<Order>(new BadRequestError(message));
        }

        order.TotalAmount = customerCart.Sum(cart => cart.MenuItem.DiscountPrice * cart.Quantity);

        order.OrderDetails = customerCart.Select(cart => new OrderDetail
        {
            OrderId = order.OrderId,
            MenuItemId = cart.MenuItemId,
            Price = cart.MenuItem.DiscountPrice,
            Quantity = cart.Quantity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

        //await _cacheService.RemoveAsync(key, cancellationToken);

        Guid shipperId = await GetNearestShipper(order);
        if (shipperId == Guid.Empty)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<Order>(new NotFoundError(message));
        }

        order.ShipperId = shipperId;
        order.CreatedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        await _orderRepository.AddAsync(order);

        return order;
    }

    private async Task<Guid> GetNearestShipper(Order order)
    {
        IEnumerable<Shipper> shippers = await _shipperRepository.ListAllAsync();
        if (!shippers.Any())
        {
            return Guid.Empty;
        }

        Shipper? nearestShipper = null;

        nearestShipper = shippers.FirstOrDefault(s => s.Address == order.Address && s.Ward == order.Ward &&
            s.District == order.District && s.City == order.City);
        if (nearestShipper != null)
        {
            return nearestShipper.ShipperId;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.Ward == order.Ward && s.District == order.District &&
            s.City == order.City);
        if (nearestShipper != null)
        {
            return nearestShipper.ShipperId;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.District == order.District && s.City == order.City);
        if (nearestShipper != null)
        {
            return nearestShipper.ShipperId;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.City == order.City);
        if (nearestShipper != null)
        {
            return nearestShipper.ShipperId;
        }

        return shippers.First().ShipperId;
    }
}
