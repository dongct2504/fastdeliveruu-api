using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRestaurantCommandHandler> _logger;
    private readonly ICacheService _cacheService;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteRestaurantCommandHandler(
        IFileStorageServices fileStorageServices,
        ICacheService cacheService,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteRestaurantCommandHandler> logger)
    {
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _unitOfWork.Restaurants.GetAsync(request.Id);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(restaurant.PublicId);
        if (deletionResult.Error != null)
        {
            string message = deletionResult.Error.Message;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.Restaurants.Delete(restaurant);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{CacheConstants.Restaurant}-{request.Id}", cancellationToken);

        return Result.Ok();
    }
}
