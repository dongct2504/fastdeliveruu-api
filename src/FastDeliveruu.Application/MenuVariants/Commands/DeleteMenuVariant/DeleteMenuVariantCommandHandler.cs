using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Commands.DeleteMenuVariant;

public class DeleteMenuVariantCommandHandler : IRequestHandler<DeleteMenuVariantCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteMenuVariantCommandHandler> _logger;

    public DeleteMenuVariantCommandHandler(
        ILogger<DeleteMenuVariantCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuVariant? menuVariant = await _unitOfWork.MenuVariants
            .GetWithSpecAsync(new MenuVariantWithOrderDetailByIdSpecification(request.Id));
        if (menuVariant == null)
        {
            string message = "MenuVariant does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        // manually set null for order detail
        foreach (OrderDetail orderDetail in menuVariant.OrderDetails)
        {
            orderDetail.MenuVariantId = null;
        }

        // delete those shopping carts that have menu variant
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";
        List<ShoppingCartDto>? shoppingCartDtos = await _cacheService.GetAsync<List<ShoppingCartDto>>(key);
        if (shoppingCartDtos != null)
        {
            List<ShoppingCartDto> shoppingCartDtosWithoutMenuVariant = shoppingCartDtos
                .Where(sc => sc.MenuVariantId != menuVariant.Id)
                .ToList();
            await _cacheService.SetAsync(key, shoppingCartDtosWithoutMenuVariant, CacheOptions.CartExpiration, cancellationToken);
        }

        _unitOfWork.MenuVariants.Delete(menuVariant);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
