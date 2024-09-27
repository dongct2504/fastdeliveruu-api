using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Cities.Queries.GetCityById;

public class GetCityByIdQuery : IRequest<Result<CityDetailDto>>
{
    public GetCityByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
