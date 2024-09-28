using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using MediatR;

namespace FastDeliveruu.Application.Wards.Queries.GetAllWards;

public class GetAllWardsQuery : IRequest<PagedList<WardDto>>
{
    public GetAllWardsQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
