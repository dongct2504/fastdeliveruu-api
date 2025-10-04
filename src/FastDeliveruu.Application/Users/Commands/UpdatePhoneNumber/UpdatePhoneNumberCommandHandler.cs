using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.UpdatePhoneNumber;

public class UpdatePhoneNumberCommandHandler : IRequestHandler<UpdatePhoneNumberCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UpdatePhoneNumberCommandHandler> _logger;

    public UpdatePhoneNumberCommandHandler(
        UserManager<AppUser> userManager,
        ILogger<UpdatePhoneNumberCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        AppUser? appUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (appUser == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        AppUser? duplicatePhoneNumber = await _userManager.Users
            .Where(u => u.PhoneNumber == request.PhoneNumber)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (duplicatePhoneNumber != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.PhoneDuplicated} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.PhoneDuplicated));
        }

        appUser.PhoneNumber = request.PhoneNumber;
        appUser.PhoneNumberConfirmed = false;

        await _userManager.UpdateAsync(appUser);

        return Result.Ok();
    }
}
