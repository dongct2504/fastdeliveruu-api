﻿using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Queries.GetMenuVariantById;

public class GetMenuVariantByIdQueryHandler : IRequestHandler<GetMenuVariantByIdQuery, Result<MenuVariantDto>>
{
    private readonly FastDeliveruuDbContext _fastDeliveruuDbContext;
    private readonly ILogger<GetMenuVariantByIdQueryHandler> _logger;

    public GetMenuVariantByIdQueryHandler(
        FastDeliveruuDbContext fastDeliveruuDbContext,
        ILogger<GetMenuVariantByIdQueryHandler> logger)
    {
        _fastDeliveruuDbContext = fastDeliveruuDbContext;
        _logger = logger;
    }

    public async Task<Result<MenuVariantDto>> Handle(GetMenuVariantByIdQuery request, CancellationToken cancellationToken)
    {
        MenuVariantDto? menuVariantDto = await _fastDeliveruuDbContext.MenuVariants
            .AsNoTracking()
            .ProjectToType<MenuVariantDto>()
            .FirstOrDefaultAsync(mv => mv.Id == request.Id, cancellationToken);

        if (menuVariantDto == null)
        {
            string message = "MenuVariant does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return menuVariantDto;
    }
}