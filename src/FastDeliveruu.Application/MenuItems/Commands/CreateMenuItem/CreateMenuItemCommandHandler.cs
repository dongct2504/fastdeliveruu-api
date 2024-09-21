using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateMenuItemCommandHandler> _logger;

    public CreateMenuItemCommandHandler(
        IFileStorageServices fileStorageServices,
        IMapper mapper,
        ILogger<CreateMenuItemCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _fileStorageServices = fileStorageServices;
        _mapper = mapper;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MenuItemDto>> Handle(
        CreateMenuItemCommand request,
        CancellationToken cancellationToken)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(request.GenreId);
        if (genre == null)
        {
            string message = "Genre not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        Restaurant? restaurant = await _unitOfWork.Restaurants.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        var spec = new MenuItemExistInRestaurantSpecification(request.RestaurantId, request.Name);

        MenuItem? menuItem = await _unitOfWork.MenuItems.GetWithSpecAsync(spec, asNoTracking: true);
        if (menuItem != null)
        {
            string message = "MenuItem is already exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
        }

        menuItem = _mapper.Map<MenuItem>(request);
        menuItem.Id = Guid.NewGuid();

        UploadResult uploadResult = await _fileStorageServices
            .UploadImageAsync(request.ImageFile, UploadPath.MenuItemImageUploadPath);
        if (uploadResult.Error != null)
        {
            string message = uploadResult.Error.Message;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        menuItem.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        menuItem.PublicId = uploadResult.PublicId;

        menuItem.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.MenuItems.Add(menuItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuItemDto>(menuItem);
    }
}
