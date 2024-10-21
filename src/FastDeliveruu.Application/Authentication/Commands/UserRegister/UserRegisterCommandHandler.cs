using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Commands.UserRegister;

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result<UserAuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RoleManager<AppRole> _roleManager; //private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<UserRegisterCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UserRegisterCommandHandler(
        IMapper mapper,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<UserRegisterCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserAuthenticationResponse>> Handle(
        UserRegisterCommand request,
        CancellationToken cancellationToken)
    {
        AppUser user = _mapper.Map<AppUser>(request);
        user.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

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

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            string message = "city does not exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }
        addressesCustomer.CityId = request.CityId;

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            string message = "district does not exist in city.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }
        addressesCustomer.DistrictId = request.DistrictId;

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            string message = "ward does not exist in district.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }
        addressesCustomer.WardId = request.WardId;

        user.AddressesCustomers.Add(addressesCustomer);

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join("\n", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string[] roleNames = { RoleConstants.Customer, RoleConstants.Staff, RoleConstants.Admin };
        foreach (string roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                //await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                await _roleManager.CreateAsync(new AppRole(roleName));
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

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return _mapper.Map<UserAuthenticationResponse>((user, encodedToken));
    }
}