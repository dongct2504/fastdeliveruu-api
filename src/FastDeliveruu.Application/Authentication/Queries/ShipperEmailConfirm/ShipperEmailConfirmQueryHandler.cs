using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FastDeliveruu.Application.Authentication.Queries.ShipperEmailConfirm;

public class ShipperEmailConfirmQueryHandler : IRequestHandler<ShipperEmailConfirmQuery, Result>
{
    private readonly UserManager<Shipper> _userManager;
    private readonly ILogger<ShipperEmailConfirmQueryHandler> _logger;

    public ShipperEmailConfirmQueryHandler(UserManager<Shipper> userManager, ILogger<ShipperEmailConfirmQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(ShipperEmailConfirmQuery request, CancellationToken cancellationToken)
    {
        Shipper? shipper = await _userManager.FindByEmailAsync(request.Email);
        if (shipper == null)
        {
            string message = "Shipper not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        string decodedToken = System.Text.Encoding.UTF8
            .GetString(WebEncoders.Base64UrlDecode(request.EnCodedToken));
        IdentityResult result = await _userManager.ConfirmEmailAsync(shipper, decodedToken);

        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join("\n", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        return Result.Ok();
    }
}
