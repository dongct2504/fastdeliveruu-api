using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using MediatR;

namespace FastDeliveruu.Application.Districts.Queries.GetAllDistricts;

public class GetAllDistrictsQuery : IRequest<PagedList<DistrictDto>>
{
    public GetAllDistrictsQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
