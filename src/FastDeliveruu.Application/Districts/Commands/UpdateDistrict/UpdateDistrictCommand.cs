using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Districts.Commands.UpdateDistrict;

public class UpdateDistrictCommand : IRequest<Result>
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Name { get; set; } = null!;
}
