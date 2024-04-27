using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Shippers.Queries.GetShipperById;

public class GetShipperByIdQueryHandler : IRequestHandler<GetShipperByIdQuery, Result<ShipperDto>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IMapper _mapper;

    public GetShipperByIdQueryHandler(IShipperRepository shipperRepository, IMapper mapper)
    {
        _shipperRepository = shipperRepository;
        _mapper = mapper;
    }

    public async Task<Result<ShipperDto>> Handle(GetShipperByIdQuery request, CancellationToken cancellationToken)
    {
        Shipper? shipper = await _shipperRepository.GetAsync(request.Id);
        if (shipper == null)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<ShipperDto>(new NotFoundError(message));
        }

        return _mapper.Map<ShipperDto>(shipper);
    }
}