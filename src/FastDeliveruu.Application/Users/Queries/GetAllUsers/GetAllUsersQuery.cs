using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PaginationResponse<LocalUserDto>>
{
    public GetAllUsersQuery(int pageNumber)
    {
        PageNumber = pageNumber;
    }

    public int PageNumber { get; }
}