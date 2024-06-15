using FastDeliveruu.Application.Dtos.AppUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<AppUserDetailDto>>
{
    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}