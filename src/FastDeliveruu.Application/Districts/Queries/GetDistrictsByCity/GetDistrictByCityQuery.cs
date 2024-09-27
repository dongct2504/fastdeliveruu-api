using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Districts.Queries.GetDistrictsByCity;

public class GetDistrictsByCityQuery : IRequest<Result<PagedList<DistrictDto>>>
{
    public GetDistrictsByCityQuery(DistrictParams districtParams)
    {
        DistrictParams = districtParams;
    }

    public DistrictParams DistrictParams { get; }
}
