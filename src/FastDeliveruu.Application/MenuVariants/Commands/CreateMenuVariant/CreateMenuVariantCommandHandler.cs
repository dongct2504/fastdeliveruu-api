using CloudinaryDotNet.Actions;
using FastDeliveruu.Common.Constants;
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

namespace FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;

public class CreateMenuVariantCommandHandler : IRequestHandler<CreateMenuVariantCommand, Result<MenuVariantDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ILogger<CreateMenuVariantCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateMenuVariantCommandHandler(
        IMapper mapper,
        ILogger<CreateMenuVariantCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFastDeliveruuUnitOfWork unitOfWork,
        IFileStorageServices fileStorageServices)
    {
        _mapper = mapper;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result<MenuVariantDto>> Handle(CreateMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        MenuVariant? menuVariant = await _unitOfWork.MenuVariants
            .GetWithSpecAsync(new MenuVariantNameExistInMenuItemSpecification(request.MenuItemId, request.VarietyName), true);
        if (menuVariant != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityDuplicate));
        }

        menuVariant = _mapper.Map<MenuVariant>(request);
        menuVariant.Id = Guid.NewGuid();
        menuVariant.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        UploadResult uploadResult = await _fileStorageServices
            .UploadImageAsync(request.ImageFile, UploadPath.MenuVariantImageUploadPath);
        if (uploadResult.Error != null)
        {
            string message = uploadResult.Error.Message;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        menuVariant.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        menuVariant.PublicId = uploadResult.PublicId;

        _unitOfWork.MenuVariants.Add(menuVariant);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuVariantDto>(menuVariant);
    }
}
