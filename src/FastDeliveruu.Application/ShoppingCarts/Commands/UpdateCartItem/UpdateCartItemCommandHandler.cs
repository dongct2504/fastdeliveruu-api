using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<int>>
{
    private readonly ICacheService _cacheService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCartItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateCartItemCommandHandler(
        IMapper mapper,
        ICacheService cacheService,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateCartItemCommandHandler> logger,
        UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result<int>> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        AppUser? appUser = await _userManager.FindByIdAsync(request.AppUserId.ToString());
        if (appUser == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        // check if menu variant provided
        MenuVariant? menuVariant = null;
        if (request.MenuVariantId.HasValue)
        {
            var spec = new MenuVariantIdExistInMenuItemSpecification(request.MenuItemId, request.MenuVariantId);
            menuVariant = await _unitOfWork.MenuVariants.GetWithSpecAsync(spec);

            if (menuVariant == null)
            {
                _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuVariantNotFound} - {request}");
                return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuVariantNotFound));
            }
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCartDto>? customerCartDtoCache = await _cacheService.GetAsync<List<ShoppingCartDto>>(key, cancellationToken);

        if (customerCartDtoCache == null)
        {
            // cart doesn't exist yet, create a new one
            ShoppingCart cartItem = _mapper.Map<ShoppingCart>(request);
            cartItem.Id = Guid.NewGuid();
            cartItem.MenuItem = menuItem;

            if (menuVariant != null)
            {
                cartItem.MenuVariant = menuVariant;
            }

            List<ShoppingCartDto> cartItemDtos = new List<ShoppingCartDto>
            {
                _mapper.Map<ShoppingCartDto>(cartItem)
            };

            await _cacheService.SetAsync(key, cartItemDtos, CacheOptions.CartExpiration, cancellationToken);

            return cartItem.Quantity;
        }

        // check if the item already exists in the cart (check both menu item and menu variant)
        ShoppingCartDto? shoppingCartDto = customerCartDtoCache.FirstOrDefault(sc =>
            sc.MenuItemId == request.MenuItemId &&
            (request.MenuVariantId != null 
                ? sc.MenuVariantId == request.MenuVariantId
                : sc.MenuVariantId == null));

        if (shoppingCartDto == null) // item doesn't exist, hence create a new item and add it to the cart
        {
            ShoppingCart newCartItem = _mapper.Map<ShoppingCart>(request);
            newCartItem.Id = Guid.NewGuid();
            newCartItem.MenuItem = menuItem;

            if (menuVariant != null)
            {
                newCartItem.MenuVariant = menuVariant;
            }

            customerCartDtoCache.Add(_mapper.Map<ShoppingCartDto>(newCartItem));
        }
        else // item exist
        {
            shoppingCartDto.Quantity += request.Quantity;
        }

        await _cacheService.SetAsync(key, customerCartDtoCache, CacheOptions.CartExpiration, cancellationToken);

        return customerCartDtoCache.Sum(cart => cart.Quantity);
    }
}
