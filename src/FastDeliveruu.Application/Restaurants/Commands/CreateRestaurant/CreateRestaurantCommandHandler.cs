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
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, Result<RestaurantDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRestaurantCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        IFastDeliveruuUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateRestaurantCommandHandler> logger)
    {
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<RestaurantDto>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var spec = new RestaurantByNameAndPhoneSpecification(request.Name, request.PhoneNumber);
        Restaurant? restaurant = await _unitOfWork.Restaurants.GetWithSpecAsync(spec, asNoTracking: true);
        if (restaurant != null)
        {
            string message = "Restaurant is already exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<RestaurantDto>(new DuplicateError(message));
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

        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
