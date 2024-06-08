using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        ICacheService cacheService)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
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
            DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(restaurant.PublicId);
            if (deletionResult.Error != null)
            {
                string message = deletionResult.Error.Message;
                Log.Warning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(request.ImageFile,
                UploadPath.RestaurantImageUploadPath);
            if (uploadResult.Error != null)
            {
                string message = uploadResult.Error.Message;
                Log.Warning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            restaurant.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            restaurant.PublicId = uploadResult.PublicId;
        }
        restaurant.UpdatedAt = DateTime.Now;

        await _restaurantRepository.UpdateAsync(restaurant);

        await _cacheService.RemoveAsync($"{CacheConstants.Restaurant}-{request.RestaurantId}", cancellationToken);

        return Result.Ok();
    }
}
