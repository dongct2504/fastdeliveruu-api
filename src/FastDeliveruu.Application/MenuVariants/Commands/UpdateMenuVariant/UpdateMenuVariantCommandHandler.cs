﻿using FastDeliveruu.Application.Common.Errors;
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
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateMenuVariantCommandHandler> _logger;

    public UpdateMenuVariantCommandHandler(
        IMapper mapper,
        ILogger<UpdateMenuVariantCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        MenuVariant? menuVariant = await _unitOfWork.MenuVariants.GetAsync(request.Id);
        if (menuVariant == null)
        {
            string message = "MenuVariant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, menuVariant);
        menuVariant.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.MenuVariants.Update(menuVariant);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}