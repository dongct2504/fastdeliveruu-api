using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Queries.GetByMenuItem;

public class GetByMenuItemQuery : IRequest<List<MenuVariantDto>>
{
    public GetByMenuItemQuery(Guid menuItemId)
    {
        MenuItemId = menuItemId;
    }

    public Guid MenuItemId { get; }
}
