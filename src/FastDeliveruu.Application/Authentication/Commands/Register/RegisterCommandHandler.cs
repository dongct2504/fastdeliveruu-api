using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IMapper mapper,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<RegisterCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        AppUser user = _mapper.Map<AppUser>(request);
        user.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join(" ", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string[] roleNames = { RoleConstants.Customer, RoleConstants.Staff, RoleConstants.Admin };
        foreach (string roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        if (request.UserName == "admin")
        {
            await _userManager.AddToRoleAsync(user, RoleConstants.Admin);
        }
        else
        {
            if (string.IsNullOrEmpty(request.Role))
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.Customer);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }
        }

        if (!string.IsNullOrEmpty(request.Address) || 
            request.CityId.HasValue || 
            request.DistrictId.HasValue ||
            request.WardId.HasValue)
        {
            AddressesCustomer addressesCustomer = new AddressesCustomer
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                IsPrimary = true,
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            };

            if (!string.IsNullOrEmpty(request.Address))
            {
                addressesCustomer.Address = request.Address;
            }
            if (request.CityId.HasValue)
            {
                addressesCustomer.CityId = request.CityId.Value;
            }
            if (request.DistrictId.HasValue)
            {
                addressesCustomer.DistrictId = request.DistrictId.Value;
            }
            if (request.WardId.HasValue)
            {
                addressesCustomer.WardId = request.WardId.Value;
            }

            user.AddressesCustomers.Add(addressesCustomer);
            await _unitOfWork.SaveChangesAsync();
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return _mapper.Map<AuthenticationResponse>((user, encodedToken));
    }
}