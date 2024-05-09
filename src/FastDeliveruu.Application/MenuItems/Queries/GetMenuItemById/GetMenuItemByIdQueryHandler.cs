using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<MenuItemDetailDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public GetMenuItemByIdQueryHandler(IMenuItemRepository menuItemRepository, IMapper mapper)
    {
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task<Result<MenuItemDetailDto>> Handle(
        GetMenuItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Genre, Restaurant",
            Where = mi => mi.MenuItemId == request.Id
        };
        MenuItem? menuItem = await _menuItemRepository.GetAsync(options);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDetailDto>(new NotFoundError(message));
        }

        return _mapper.Map<MenuItemDetailDto>(menuItem);
    }
}
