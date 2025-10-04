using CloudinaryDotNet.Actions;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, Result<RestaurantDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateRestaurantCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        IFastDeliveruuUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateRestaurantCommandHandler> logger,
        ICacheService cacheService)
    {
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result<RestaurantDto>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var spec = new RestaurantByNameAndPhoneSpecification(request.Name, request.PhoneNumber);
        Restaurant? restaurant = await _unitOfWork.Restaurants.GetWithSpecAsync(spec, asNoTracking: true);
        if (restaurant != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.RestaurantDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.RestaurantDuplicate));
        }

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }

        restaurant = _mapper.Map<Restaurant>(request);
        restaurant.Id = Guid.NewGuid();

        UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(request.ImageFile,
            UploadPath.RestaurantImageUploadPath);
        if (uploadResult.Error != null)
        {
            string message = uploadResult.Error.Message;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        restaurant.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        restaurant.PublicId = uploadResult.PublicId;

        restaurant.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Restaurants.Add(restaurant);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Restaurants, cancellationToken);

        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
