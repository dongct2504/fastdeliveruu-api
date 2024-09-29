using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
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

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            string message = "City does not exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId), true);
        if (district == null)
        {
            string message = "District does not exist in city.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId), true);
        if (ward == null)
        {
            string message = "Ward does not exist in district.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCartDto>? customerCartDto = await _cacheService.GetAsync<List<ShoppingCartDto>>(key, cancellationToken);
        if (customerCartDto == null || !customerCartDto.Any())
        {
            string message = "The customer's cart is empty.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Order order = _mapper.Map<Order>(request);
        order.Id = Guid.NewGuid();

        decimal totalAmount = 0;
        List<OrderDetail> orderDetails = new List<OrderDetail>();

        foreach (ShoppingCartDto cartItemDto in customerCartDto)
        {
            // if it has menu variant, get the price of the menu variant not menu item
            decimal itemPrice = cartItemDto.MenuVariantDto?.DiscountPrice ?? cartItemDto.MenuItemDto.DiscountPrice;

            orderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                MenuItemId = cartItemDto.MenuItemId,
                MenuVariantId = cartItemDto.MenuVariantId,
                Price = itemPrice,
                Quantity = cartItemDto.Quantity,
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            });

            totalAmount += itemPrice * cartItemDto.Quantity;
        }

        order.TotalAmount = totalAmount;
        order.OrderDetails = orderDetails;

        order.OrderDescription = $"Create payment for order: {order.Id}";
        order.OrderDate = _dateTimeProvider.VietnamDateTimeNow;
        order.TransactionId = "0";
        order.OrderStatus = (byte?)OrderStatusEnum.Pending;
        order.PaymentStatus = (byte?)PaymentStatusEnum.Pending;
        order.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Orders.Add(order);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync(key, cancellationToken);

        return order;
    }
}
