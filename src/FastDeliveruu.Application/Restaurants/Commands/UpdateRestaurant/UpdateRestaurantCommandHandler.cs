using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, Result>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, restaurant);
        
        if (request.ImageFile != null)
        {
            await _fileStorageServices.DeleteImageAsync(restaurant.ImageUrl);

            string? fileNameExtension = await _fileStorageServices.UploadImageAsync(request.ImageFile,
                UploadPath.RestaurantImageUploadPath);
            restaurant.ImageUrl = UploadPath.RestaurantImageUploadPath + fileNameExtension;
        }
        restaurant.UpdatedAt = DateTime.Now;

        await _restaurantRepository.UpdateAsync(restaurant);

        return Result.Ok();
    }
}
