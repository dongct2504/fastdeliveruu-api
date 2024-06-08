using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IFileStorageServices fileStorageServices,
        ICacheService cacheService)
    {
        _restaurantRepository = restaurantRepository;
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetAsync(request.Id);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(restaurant.PublicId);
        if (deletionResult.Error != null)
        {
            string message = deletionResult.Error.Message;
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        await _restaurantRepository.DeleteAsync(restaurant);

        await _cacheService.RemoveAsync($"{CacheConstants.Restaurant}-{request.Id}", cancellationToken);

        return Result.Ok();
    }
}
