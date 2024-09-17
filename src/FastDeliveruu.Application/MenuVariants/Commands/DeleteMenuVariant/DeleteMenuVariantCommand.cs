using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Commands.DeleteMenuVariant;

public class DeleteMenuVariantCommand : IRequest<Result>
{
    public Guid Id { get; }

    public DeleteMenuVariantCommand(Guid id)
    {
        Id = id;
    }
}
