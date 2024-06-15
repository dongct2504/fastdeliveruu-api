using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PagedList<AppUserDto>>
{
    public GetAllUsersQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}