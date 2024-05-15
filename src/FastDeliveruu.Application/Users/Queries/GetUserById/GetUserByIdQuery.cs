using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<LocalUserDetailDto>>
{
    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}