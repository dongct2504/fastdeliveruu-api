using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Wards.Commands.CreateWard;

public class CreateWardCommand : IRequest<Result<WardDto>>
{
    public int DistrictId { get; set; }

    public string Name { get; set; } = null!;
}
