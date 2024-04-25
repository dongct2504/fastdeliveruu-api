using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.EmailConfirm;

public class EmailConfirmQuery : IRequest<Result<bool>>
{
    public EmailConfirmQuery(string email, string token)
    {
        Email = email;
        Token = token;
    }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;
}