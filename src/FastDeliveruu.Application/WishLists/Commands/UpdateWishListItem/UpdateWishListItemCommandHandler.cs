using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FastDeliveruu.Domain.Specifications.WishLists;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.WishLists.Commands.UpdateWishListItem;

public class UpdateWishListItemCommandHandler : IRequestHandler<UpdateWishListItemCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateWishListItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateWishListItemCommandHandler(
        UserManager<AppUser> userManager,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateWishListItemCommandHandler> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateWishListItemCommand request, CancellationToken cancellationToken)
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

        IEnumerable<WishList> wishListsByUser = await _unitOfWork.WishLists
            .ListAllWithSpecAsync(new WishListsByUserIdSpecification(request.AppUserId));

        WishList? wishList = wishListsByUser
            .Where(w => w.MenuItemId == request.MenuItemId &&
                (request.MenuVariantId != null 
                    ? w.MenuVariantId == request.MenuVariantId
                    : w.MenuVariantId == null))
            .FirstOrDefault();

        if (wishList == null) // item doesn't exist, hence create a new item and add it to wish list
        {
            wishList = _mapper.Map<WishList>(request);
            wishList.Id = new Guid();
            wishList.MenuItem = menuItem;

            if (menuVariant != null)
            {
                wishList.MenuVariant = menuVariant;
            }

            _unitOfWork.WishLists.Add(wishList);
            await _unitOfWork.SaveChangesAsync();
        }

        return Result.Ok();
    }
}
