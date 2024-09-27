using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Districts.Queries.GetDistrictById;

public class GetDistrictByIdQuery : IRequest<Result<DistrictDetailDto>>
{
    public GetDistrictByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
