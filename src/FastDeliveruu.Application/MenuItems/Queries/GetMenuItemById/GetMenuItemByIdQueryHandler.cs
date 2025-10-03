using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<MenuItemDetailDto>>
{
    private readonly ILogger<GetMenuItemByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetMenuItemByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetMenuItemByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<MenuItemDetailDto>> Handle(
        GetMenuItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        MenuItemDetailDto? menuItemDetailDto = await _dbContext.MenuItems
            .Where(mi => mi.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<MenuItemDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (menuItemDetailDto == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new NotFoundError(ErrorMessageConstants.MenuItemNotFound));
        }

        return menuItemDetailDto;
    }
}
