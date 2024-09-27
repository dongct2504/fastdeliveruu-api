using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Cities.Commands.DeleteCity;

public class DeleteCityCommand : IRequest<Result>
{
    public DeleteCityCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
