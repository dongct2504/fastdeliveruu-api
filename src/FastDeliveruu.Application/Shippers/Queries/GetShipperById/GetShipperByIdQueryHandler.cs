using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Shippers.Queries.GetShipperById;

public class GetShipperByIdQueryHandler : IRequestHandler<GetShipperByIdQuery, Result<ShipperDetailDto>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IMapper _mapper;

    public GetShipperByIdQueryHandler(IShipperRepository shipperRepository, IMapper mapper)
    {
        _shipperRepository = shipperRepository;
        _mapper = mapper;
    }

    public async Task<Result<ShipperDetailDto>> Handle(
        GetShipperByIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            SetIncludes = "Orders",
            Where = s => s.ShipperId == request.Id
        };
        Shipper? shipper = await _shipperRepository.GetAsync(options, asNoTracking: true);
        if (shipper == null)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return _mapper.Map<ShipperDetailDto>(shipper);
    }
}