using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.EmailConfirm;

public class EmailConfirmQuery : IRequest<Result>
{
    public EmailConfirmQuery(string email, string encodedToken)
    {
        Email = email;
        EnCodedToken = encodedToken;
    }

    public string Email { get; set; } = null!;

    public string EnCodedToken { get; set; } = null!;
}