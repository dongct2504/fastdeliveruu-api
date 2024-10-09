using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.UserEmailConfirm;

public class UserEmailConfirmQuery : IRequest<Result>
{
    public UserEmailConfirmQuery(string email, string encodedToken)
    {
        Email = email;
        EnCodedToken = encodedToken;
    }

    public string Email { get; set; } = null!;

    public string EnCodedToken { get; set; } = null!;
}