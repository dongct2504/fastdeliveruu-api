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
            string message = "User not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        AppUser? duplicatePhoneNumber = await _userManager.Users
            .Where(u => u.PhoneNumber == request.PhoneNumber)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (duplicatePhoneNumber != null)
        {
            string message = "Số điện thoại đã tồn tại.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
        }

        appUser.PhoneNumber = request.PhoneNumber;
        appUser.PhoneNumberConfirmed = false;

        await _userManager.UpdateAsync(appUser);

        return Result.Ok();
    }
}
