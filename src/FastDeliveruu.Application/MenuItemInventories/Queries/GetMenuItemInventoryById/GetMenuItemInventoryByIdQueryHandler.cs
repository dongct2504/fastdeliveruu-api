using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItemInventories.Queries.GetMenuItemInventoryById;

public class GetMenuItemInventoryByIdQueryHandler : IRequestHandler<GetMenuItemInventoryByIdQuery, Result<MenuItemInventoryDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetMenuItemInventoryByIdQueryHandler> _logger;

    public GetMenuItemInventoryByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetMenuItemInventoryByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<MenuItemInventoryDto>> Handle(GetMenuItemInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        MenuItemInventoryDto? menuItemInventoryDto = await _dbContext.MenuItemInventories
            .ProjectToType<MenuItemInventoryDto>()
            .FirstOrDefaultAsync(mi => mi.Id == request.Id);

        if (menuItemInventoryDto == null)
        {
            string message = "MenuItem Inventory not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return menuItemInventoryDto;
    }
}
