using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;
    private readonly IMailNotificationService _orderNotificationService;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IMapper mapper,
        ICacheService cacheService,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        UserManager<AppUser> userManager,
        IGeocodingService geocodingService,
        IMailNotificationService orderNotificationService)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _userManager = userManager;
        _geocodingService = geocodingService;
        _orderNotificationService = orderNotificationService;
    }

    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        AppUser? appUser = await _userManager.FindByIdAsync(request.AppUserId.ToString());
        if (appUser == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        if (!appUser.PhoneNumberConfirmed)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.PhoneYetConfirmed} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.PhoneYetConfirmed));
        }

        DeliveryMethod? deliveryMethod = await _unitOfWork.DeliveryMethods.GetAsync(request.DeliveryMethodId);
        if (deliveryMethod == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DeliveryNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DeliveryNotFound));
        }

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }

        string fullAddress = $"{request.HouseNumber} {request.StreetName}, {ward.Name}, {district.Name}, {city.Name}";
        (double, double)? accurateLocation = await _geocodingService.ConvertToLatLongAsync(fullAddress);
        if (accurateLocation == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.LatLongNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.LatLongNotFound));
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCartDto>? customerCartDto = await _cacheService.GetAsync<List<ShoppingCartDto>>(key, cancellationToken);
        if (customerCartDto == null || !customerCartDto.Any())
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CustomerCartEmpty} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CustomerCartEmpty));
        }

        Order order = _mapper.Map<Order>(request);
        order.Id = Guid.NewGuid();
        order.PhoneNumber = appUser.PhoneNumber;

        order.Latitude = (decimal)accurateLocation.Value.Item1;
        order.Longitude = (decimal)accurateLocation.Value.Item2;

        decimal totalAmount = 0;
        List<OrderDetail> orderDetails = new List<OrderDetail>();

        foreach (ShoppingCartDto cartItemDto in customerCartDto)
        {
            // if it has menu variant, get the price of the menu variant not menu item
            decimal itemPrice = cartItemDto.MenuVariantDto?.DiscountPrice ?? cartItemDto.MenuItemDto.DiscountPrice;

            orderDetails.Add(new OrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                MenuItemId = cartItemDto.MenuItemId,
                MenuVariantId = cartItemDto.MenuVariantId,
                Price = itemPrice,
                Quantity = cartItemDto.Quantity,
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            });

            totalAmount += itemPrice * cartItemDto.Quantity;

            // if it's a menu item
            if (!cartItemDto.MenuVariantId.HasValue)
            {
                MenuItemInventory? menuItemInventory = await _unitOfWork.MenuItemInventories
                    .GetWithSpecAsync(new MenuItemInventoryByMenuItemIdSpecification(cartItemDto.MenuItemId));

                if (menuItemInventory == null || menuItemInventory.QuantityAvailable < cartItemDto.Quantity)
                {
                    _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemInventoryNotEnough} - {request}");
                    return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemInventoryNotEnough));
                }

                menuItemInventory.QuantityAvailable -= cartItemDto.Quantity;
                menuItemInventory.QuantityReserved += cartItemDto.Quantity;
            }
            else // if it's a menu variant
            {
                MenuVariantInventory? menuVariantInventory = await _unitOfWork.MenuVariantInventories
                    .GetWithSpecAsync(new MenuVariantInventoryByMenuVariantIdSpecification(cartItemDto.MenuVariantId.Value));

                if (menuVariantInventory == null || menuVariantInventory.QuantityAvailable < cartItemDto.Quantity)
                {
                    _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuVariantInventoryNotEnough} - {request}");
                    return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuVariantInventoryNotEnough));
                }

                menuVariantInventory.QuantityAvailable -= cartItemDto.Quantity;
                menuVariantInventory.QuantityReserved += cartItemDto.Quantity;
            }
        }

        order.TotalAmount = totalAmount + deliveryMethod.Price;
        order.OrderDetails = orderDetails;

        // paypal setting
        if (request.PaymentMethod == PaymentMethodsEnum.Paypal)
        {
            decimal totalAmountUSD = order.TotalAmount * (decimal)0.000042;
            request.Amount = totalAmountUSD.ToString("F2");
            request.Reference = order.Id.ToString();
        }

        order.OrderDescription = $"Create payment for order: {order.Id}";
        order.OrderDate = _dateTimeProvider.VietnamDateTimeNow;
        order.TransactionId = "0";
        order.OrderStatus = (byte?)OrderStatusEnum.Pending;
        order.PaymentMethod = (byte?)request.PaymentMethod;
        order.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        // Default shipping status for new order
        order.DeliveryMethodId = 1; // 1 = Đang giao hàng (per requirement)

        Shipper? shipper = await _unitOfWork.Shippers.FindNearestShipper(order.Latitude, order.Longitude);
        if (shipper == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.ShipperNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.ShipperNotFound));
        }
        order.ShipperId = shipper.Id;

        _unitOfWork.Orders.Add(order);

        Payment payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentStatus = (byte?)PaymentStatusEnum.Pending,
            PaymentMethod = (byte?)request.PaymentMethod,
            TransactionId = "0",
            CreatedAt = _dateTimeProvider.VietnamDateTimeNow
        };

        _unitOfWork.Payments.Add(payment);

        await _orderNotificationService.SendOrderNotificationAsync(
            appUser,
            order,
            (OrderStatusEnum)order.OrderStatus,
            (PaymentMethodsEnum)order.PaymentMethod,
            (PaymentStatusEnum)payment.PaymentStatus
        );

        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveAsync(key, cancellationToken);

        return order;
    }
}
