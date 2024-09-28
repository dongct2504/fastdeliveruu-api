using FastDeliveruu.Application.Dtos.AddressDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Wards.Queries.GetWardById;

public class GetWardByIdQuery : IRequest<Result<WardDto>>
{
    public GetWardByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
