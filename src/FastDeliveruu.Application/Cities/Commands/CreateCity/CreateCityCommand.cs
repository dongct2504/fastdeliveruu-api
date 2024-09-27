using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Cities.Commands.CreateCity;

public class CreateCityCommand : IRequest<Result<CityDto>>
{
    public CreateCityCommand(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
