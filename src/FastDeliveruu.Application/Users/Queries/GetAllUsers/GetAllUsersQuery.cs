using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PagedList<AppUserDto>>
{
    public GetAllUsersQuery(AppUserParams appUserParams)
    {
        AppUserParams = appUserParams;
    }

    public AppUserParams AppUserParams { get; }
}