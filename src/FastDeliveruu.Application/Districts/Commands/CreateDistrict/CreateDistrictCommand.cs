using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Districts.Commands.CreateDistrict;

public class CreateDistrictCommand : IRequest<Result<DistrictDto>>
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;
}
