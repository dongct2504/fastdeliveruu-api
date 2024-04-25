using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.Login;

public class LoginQuery : IRequest<Result<AuthenticationResponse>>
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}