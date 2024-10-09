using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.ShipperEmailConfirm;

public class ShipperEmailConfirmQuery : IRequest<Result>
{
    public ShipperEmailConfirmQuery(string email, string encodedToken)
    {
        Email = email;
        EnCodedToken = encodedToken;
    }

    public string Email { get; set; } = null!;

    public string EnCodedToken { get; set; } = null!;
}
