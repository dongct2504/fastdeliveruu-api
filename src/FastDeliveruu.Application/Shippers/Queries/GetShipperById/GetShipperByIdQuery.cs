using FastDeliveruu.Application.Dtos.ShipperDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Shippers.Queries.GetShipperById;

public class GetShipperByIdQuery : IRequest<Result<ShipperDetailDto>>
{
    public GetShipperByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}