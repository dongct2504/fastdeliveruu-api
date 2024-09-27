using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using MediatR;

namespace FastDeliveruu.Application.Cities.Queries.GetAllCities;

public class GetAllCitiesQuery : IRequest<PagedList<CityDto>>
{
    public GetAllCitiesQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
