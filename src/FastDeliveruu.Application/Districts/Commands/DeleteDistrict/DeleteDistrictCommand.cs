using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Districts.Commands.DeleteDistrict;

public class DeleteDistrictCommand : IRequest<Result>
{
    public DeleteDistrictCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
