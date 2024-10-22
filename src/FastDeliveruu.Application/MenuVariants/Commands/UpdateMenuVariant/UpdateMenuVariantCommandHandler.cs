using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;

public class UpdateMenuVariantCommandHandler : IRequestHandler<UpdateMenuVariantCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateMenuVariantCommandHandler> _logger;

    public UpdateMenuVariantCommandHandler(
        IMapper mapper,
        ILogger<UpdateMenuVariantCommandHandler> logger,
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

    public async Task<Result> Handle(UpdateMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        MenuVariant? menuVariant = await _unitOfWork.MenuVariants.GetAsync(request.Id);
        if (menuVariant == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuVariantNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuVariantNotFound));
        }

        _mapper.Map(request, menuVariant);
        menuVariant.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        if (request.ImageFile != null)
        {
            DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(menuVariant.PublicId);
            if (deletionResult.Error != null)
            {
                string message = deletionResult.Error.Message;
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(request.ImageFile, UploadPath.MenuVariantImageUploadPath);
            if (uploadResult.Error != null)
            {
                string message = uploadResult.Error.Message;
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            menuVariant.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            menuVariant.PublicId = uploadResult.PublicId;
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
