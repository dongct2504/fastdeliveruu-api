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
    private readonly IDeliveryMethodRepository _deliveryMethodRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IMapper mapper,
        ICacheService cacheService,
        IDeliveryMethodRepository deliveryMethodRepository)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _deliveryMethodRepository = deliveryMethodRepository;
    }

    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        DeliveryMethod? deliveryMethod = await _deliveryMethodRepository.GetAsync(request.DeliveryMethodId);
        if (deliveryMethod == null)
        {
            string message = "The delivery method does not exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<Order>(new BadRequestError(message));
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCart>? customerCart = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);
        if (customerCart == null || !customerCart.Any())
        {
            string message = "The customer's cart is empty.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<Order>(new BadRequestError(message));
        }

        Order order = _mapper.Map<Order>(request);
        order.Id = Guid.NewGuid();
        order.OrderDescription = $"Create payment for order: {order.Id}";
        order.OrderDate = DateTime.Now;
        order.TransactionId = "0";
        order.OrderStatus = (byte?)OrderStatus.Pending;
        order.PaymentStatus = (byte?)PaymentStatus.Pending;

        order.TotalAmount = customerCart.Sum(cart => cart.MenuItem.DiscountPrice * cart.Quantity) + deliveryMethod.Price;

        order.OrderDetails = customerCart.Select(cart => new OrderDetail
        {
            OrderId = order.Id,
            MenuItemId = cart.MenuItemId,
            Price = cart.MenuItem.DiscountPrice,
            Quantity = cart.Quantity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

        order.CreatedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        await _orderRepository.AddAsync(order);

        await _cacheService.RemoveAsync(key, cancellationToken);

        return order;
    }
}
