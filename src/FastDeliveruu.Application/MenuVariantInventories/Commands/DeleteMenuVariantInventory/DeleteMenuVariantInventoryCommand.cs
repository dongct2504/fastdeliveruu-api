using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariantInventories.Commands.DeleteMenuVariantInventory;

public class DeleteMenuVariantInventoryCommand : IRequest<Result>
{
    public DeleteMenuVariantInventoryCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
