using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, Result>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IFileStorageServices fileStorageServices)
    {
        _restaurantRepository = restaurantRepository;
        _fileStorageServices = fileStorageServices;
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

        await _restaurantRepository.DeleteAsync(restaurant);

        await _fileStorageServices.DeleteImageAsync(restaurant.ImageUrl);

        return Result.Ok();
    }
}
