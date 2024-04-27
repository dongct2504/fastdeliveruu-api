using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Shippers.Commands.DeleteShipper;

public class DeleteShipperCommand : IRequest<Result>
{
    public DeleteShipperCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}