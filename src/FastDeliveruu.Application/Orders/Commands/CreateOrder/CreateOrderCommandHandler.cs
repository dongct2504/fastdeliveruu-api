using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IShipperRepository _shipperRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IShoppingCartRepository shoppingCartRepository,
        IShipperRepository shipperRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _shipperRepository = shipperRepository;
        _mapper = mapper;
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

        QueryOptions<ShoppingCart> shoppingCartOptions = new QueryOptions<ShoppingCart>
        {
            SetIncludes = "MenuItem",
            Where = sc => sc.LocalUserId == request.LocalUserId
        };
        IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartRepository
            .ListAllAsync(shoppingCartOptions, asNoTracking: true);
        if (!shoppingCarts.Any())
        {
            string message = "Cart is empty.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<Order>(new BadRequestError(message));
        }

        order.OrderDetails = shoppingCarts.Select(cart => new OrderDetail
        {
            OrderId = order.OrderId,
            MenuItemId = cart.MenuItemId,
            Price = cart.MenuItem.Price,
            Quantity = cart.Quantity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

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
