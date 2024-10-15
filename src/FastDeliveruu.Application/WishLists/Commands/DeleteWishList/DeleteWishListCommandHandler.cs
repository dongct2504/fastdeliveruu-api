﻿using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.WishLists;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.WishLists.Commands.DeleteWishList;

public class DeleteWishListCommandHandler : IRequestHandler<DeleteWishListCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteWishListCommandHandler> _logger;

    public DeleteWishListCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteWishListCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteWishListCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<WishList> wishLists = await _unitOfWork.WishLists
            .ListAllWithSpecAsync(new WishListsByUserIdSpecification(request.UserId));

        if (!wishLists.Any())
        {
            string message = "The customer's wish list is already empty";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.WishLists.DeleteRange(wishLists);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
