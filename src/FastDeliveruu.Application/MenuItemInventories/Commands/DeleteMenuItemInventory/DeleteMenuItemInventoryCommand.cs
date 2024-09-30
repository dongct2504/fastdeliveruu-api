using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuItemInventories.Commands.DeleteMenuItemInventory;

public class DeleteMenuItemInventoryCommand : IRequest<Result>
{
    public DeleteMenuItemInventoryCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
