using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.ForgotPassword;

public class ForgotPasswordQuery : IRequest<Result>
{
    public ForgotPasswordQuery(string email)
    {
        Email = email;
    }

    public string Email { get; set; } = null!;
}
