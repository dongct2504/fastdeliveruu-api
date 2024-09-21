using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IMapper mapper,
        ICacheService cacheService,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        DeliveryMethod? deliveryMethod = await _unitOfWork.DeliveryMethods.GetAsync(request.DeliveryMethodId);
        if (deliveryMethod == null)
        {
            string message = "The delivery method does not exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCart>? customerCart = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);
        if (customerCart == null || !customerCart.Any())
        {
            string message = "The customer's cart is empty.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Order order = _mapper.Map<Order>(request);
        order.Id = Guid.NewGuid();
        order.OrderDescription = $"Create payment for order: {order.Id}";
        order.OrderDate = _dateTimeProvider.VietnamDateTimeNow;
        order.TransactionId = "0";
        order.OrderStatus = (byte?)OrderStatus.Pending;
        order.PaymentStatus = (byte?)PaymentStatus.Pending;

        order.TotalAmount = customerCart.Sum(cart => cart.MenuItem.DiscountPrice * cart.Quantity) + deliveryMethod.Price;

        order.OrderDetails = customerCart.Select(cart => new OrderDetail
        {
            OrderId = order.Id,
            MenuItemId = cart.MenuItemId,
            MenuVariantId = cart.MenuVariantId,
            Price = cart.MenuItem.DiscountPrice,
            Quantity = cart.Quantity,
            CreatedAt = _dateTimeProvider.VietnamDateTimeNow
        }).ToList();

        order.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Orders.Add(order);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync(key, cancellationToken);

        return order;
    }
}
