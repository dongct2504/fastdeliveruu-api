using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Wards.Commands.DeleteWard;

public class DeleteWardCommand : IRequest<Result>
{
    public DeleteWardCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
