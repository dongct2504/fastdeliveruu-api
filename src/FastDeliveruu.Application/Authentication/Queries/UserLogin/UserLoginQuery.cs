using FastDeliveruu.Application.Dtos.AppUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.UserLogin;

public class UserLoginQuery : IRequest<Result<UserAuthenticationResponse>>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}