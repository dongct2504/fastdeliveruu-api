using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommand : IRequest<Result>
{
    public DeleteMenuItemCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
