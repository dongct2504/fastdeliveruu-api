using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.CreateShoppingCart;

public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, Result>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public CreateShoppingCartCommandHandler(
        IShoppingCartRepository shoppingCartRepository,
        IMapper mapper,
        ILocalUserRepository localUserRepository,
        IMenuItemRepository menuItemRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _mapper = mapper;
        _localUserRepository = localUserRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<Result> Handle(
        CreateShoppingCartCommand request,
        CancellationToken cancellationToken)
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
        if (shoppingCart != null)
        {
            shoppingCart.Quantity += request.Quantity;
            shoppingCart.UpdatedAt = DateTime.Now;
            await _shoppingCartRepository.UpdateAsync(shoppingCart);
            return Result.Ok();
        }

        shoppingCart = _mapper.Map<ShoppingCart>(request);
        shoppingCart.CreatedAt = DateTime.Now;
        shoppingCart.UpdatedAt = DateTime.Now;

        await _shoppingCartRepository.AddAsync(shoppingCart);

        return Result.Ok();
    }
}
