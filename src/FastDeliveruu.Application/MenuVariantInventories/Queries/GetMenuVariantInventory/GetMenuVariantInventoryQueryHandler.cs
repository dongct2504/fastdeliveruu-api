using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariantInventories.Queries.GetMenuVariantInventory;

public class GetMenuVariantInventoryQueryHandler : IRequestHandler<GetMenuVariantInventoryQuery, Result<MenuVariantInventoryDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetMenuVariantInventoryQueryHandler> _logger;

    public GetMenuVariantInventoryQueryHandler(FastDeliveruuDbContext dbContext, ILogger<GetMenuVariantInventoryQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<MenuVariantInventoryDto>> Handle(GetMenuVariantInventoryQuery request, CancellationToken cancellationToken)
    {
        MenuVariantInventoryDto? menuVariantInventoryDto = await _dbContext.MenuVariantInventories
            .ProjectToType<MenuVariantInventoryDto>()
            .FirstOrDefaultAsync(mi => mi.Id == request.Id);
        
        if (menuVariantInventoryDto == null)
        {
            string message = "MenuVariant Inventory not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return menuVariantInventoryDto;
    }
}
