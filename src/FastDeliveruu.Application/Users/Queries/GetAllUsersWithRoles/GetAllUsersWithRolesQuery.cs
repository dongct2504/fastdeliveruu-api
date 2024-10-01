using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsersWithRoles;

public class GetAllUsersWithRolesQuery : IRequest<PagedList<AppUserWithRolesDto>>
{
    public GetAllUsersWithRolesQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
