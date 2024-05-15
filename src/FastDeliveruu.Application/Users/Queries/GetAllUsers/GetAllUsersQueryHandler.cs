using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler :
    IRequestHandler<GetAllUsersQuery, PaginationResponse<LocalUserDto>>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        ILocalUserRepository localUserRepository,
        IMapper mapper)
    {
        _localUserRepository = localUserRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<LocalUserDto>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<LocalUser> options = new QueryOptions<LocalUser>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.UserPageSize
        };

        PaginationResponse<LocalUserDto> paginationResponse = new PaginationResponse<LocalUserDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.UserPageSize,
            Items = _mapper.Map<IEnumerable<LocalUserDto>>(
                await _localUserRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _localUserRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}