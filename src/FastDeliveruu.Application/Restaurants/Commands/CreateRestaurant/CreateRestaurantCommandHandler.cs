using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, Result<RestaurantDto>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result<RestaurantDto>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var spec = new RestaurantByNameAndPhoneSpecification(request.Name, request.PhoneNumber);
        Restaurant? restaurant = await _restaurantRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (restaurant != null)
        {
            string message = "Restaurant is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<RestaurantDto>(new DuplicateError(message));
        }

        restaurant = _mapper.Map<Restaurant>(request);
        restaurant.Id = Guid.NewGuid();

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

        restaurant.CreatedAt = DateTime.Now;
        restaurant.UpdatedAt = DateTime.Now;

        await _restaurantRepository.AddAsync(restaurant);

        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
