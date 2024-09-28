using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Wards.Queries.GetWardsByDistrict;

public class GetWardsByDistrictQuery : IRequest<Result<PagedList<WardDto>>>
{
    public GetWardsByDistrictQuery(WardParams wardParams)
    {
        WardParams = wardParams;
    }

    public WardParams WardParams { get; }
}
