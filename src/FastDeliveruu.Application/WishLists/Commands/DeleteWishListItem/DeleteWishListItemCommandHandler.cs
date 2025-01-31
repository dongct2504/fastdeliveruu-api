﻿using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.WishLists.Commands.DeleteWishListItem;

public class DeleteWishListItemCommandHandler : IRequestHandler<DeleteWishListItemCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteWishListItemCommandHandler> _logger;

    public DeleteWishListItemCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteWishListItemCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteWishListItemCommand request, CancellationToken cancellationToken)
    {
        WishList? wishList = await _unitOfWork.WishLists.GetAsync(request.Id);
        if (wishList == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WishListNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WishListNotFound));
        }

        _unitOfWork.WishLists.Delete(wishList);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
