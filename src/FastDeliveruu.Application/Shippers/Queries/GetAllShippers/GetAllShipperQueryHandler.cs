using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Shippers.Queries.GetAllShippers;

public class GetAllShipperQueryHandler : IRequestHandler<GetAllShippersQuery, PaginationResponse<ShipperDto>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IMapper _mapper;

    public GetAllShipperQueryHandler(IShipperRepository shipperRepository, IMapper mapper)
    {
        _shipperRepository = shipperRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<ShipperDto>> Handle(
        GetAllShippersQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.UserPageSize
        };

        PaginationResponse<ShipperDto> paginationResponse = new PaginationResponse<ShipperDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.UserPageSize,
            TotalRecords = await _shipperRepository.GetCountAsync(),
            Items = _mapper.Map<IEnumerable<ShipperDto>>(await _shipperRepository.ListAllAsync(options))
        };

        return paginationResponse;
    }
}