using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Wards.Commands.UpdateWard;

public class UpdateWardCommand : IRequest<Result>
{
    public int Id { get; set; }

    public int DistrictId { get; set; }

    public string Name { get; set; } = null!;
}
