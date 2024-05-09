using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQuery : IRequest<Result<MenuItemDetailDto>>
{
    public GetMenuItemByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
