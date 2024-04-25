using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.EmailConfirm;

public class EmailConfirmQueryHandler : IRequestHandler<EmailConfirmQuery, Result<bool>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly ILocalUserRepository _localUserRepository;

    public EmailConfirmQueryHandler(
        IShipperRepository shipperRepository,
        ILocalUserRepository localUserRepository)
    {
        _shipperRepository = shipperRepository;
        _localUserRepository = localUserRepository;
    }

    public async Task<Result<bool>> Handle(EmailConfirmQuery request, CancellationToken cancellationToken)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(request.Token);

        // Retrieve the user ID and email from the token
        Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        Claim? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

        if (userIdClaim == null || emailClaim == null)
        {
            return Result.Fail<bool>(new BadRequestError("Invalid token format."));
        }

        // Retrieve the user based on email
        QueryOptions<LocalUser> LocalUserOptions = new QueryOptions<LocalUser>
        {
            Where = u => u.Email == request.Email
        };
        LocalUser? localUser = await _localUserRepository.GetAsync(LocalUserOptions);

        QueryOptions<Shipper> ShipperOptions = new QueryOptions<Shipper>
        {
            Where = s => s.Email == request.Email
        };
        Shipper? shipper = await _shipperRepository.GetAsync(ShipperOptions);

        if (localUser == null && shipper == null)
        {
            return Result.Fail<bool>(new NotFoundError("User not found."));
        }

        if (localUser != null)
        {
            // Verify the email confirmation token
            if (emailClaim.Value != request.Email)
            {
                return Result.Fail<bool>(new BadRequestError("Email not match."));
            }

            // Update the user's email confirmation status
            localUser.IsConfirmEmail = true;
            localUser.UpdatedAt = DateTime.Now;

            await _localUserRepository.UpdateAsync(localUser);
        }

        if (shipper != null)
        {
            // Verify the email confirmation token
            if (emailClaim.Value != request.Email)
            {
                return Result.Fail<bool>(new BadRequestError("Email not match."));
            }

            // Update the user's email confirmation status
            shipper.IsConfirmEmail = true;
            shipper.UpdatedAt = DateTime.Now;

            await _shipperRepository.UpdateAsync(shipper);
        }

        return true;
    }
}