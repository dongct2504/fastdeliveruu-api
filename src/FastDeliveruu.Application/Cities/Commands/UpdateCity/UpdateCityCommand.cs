using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Cities.Commands.UpdateCity;

public class UpdateCityCommand : IRequest<Result>
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
