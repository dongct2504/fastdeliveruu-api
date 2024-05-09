using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;

public class UpdateShoppingCartCommandHandler : IRequestHandler<UpdateShoppingCartCommand, Result>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public UpdateShoppingCartCommandHandler(
        IShoppingCartRepository shoppingCartRepository,
        ILocalUserRepository localUserRepository,
        IMenuItemRepository menuItemRepository,
        IMapper mapper)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _localUserRepository = localUserRepository;
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateShoppingCartCommand request, CancellationToken cancellationToken)
    {
        LocalUser? user = await _localUserRepository.GetAsync(request.LocalUserId);
        if (user == null)
        {
            string message = "User not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        MenuItem? menuItem = await _menuItemRepository.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == request.LocalUserId && sc.MenuItemId == request.MenuItemId
        };
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (shoppingCart == null)
        {
            string message = "Cart not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, shoppingCart);
        shoppingCart.UpdatedAt = DateTime.Now;

        await _shoppingCartRepository.UpdateAsync(shoppingCart);

        return Result.Ok();
    }
}
