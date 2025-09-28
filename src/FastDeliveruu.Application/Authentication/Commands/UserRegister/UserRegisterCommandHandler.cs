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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Commands.UserRegister;

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result<UserAuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    //private readonly RoleManager<AppRole> _roleManager; //private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<UserRegisterCommandHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMailNotificationService _mailNotificationService;
    private readonly IMapper _mapper;

    public UserRegisterCommandHandler(
        IMapper mapper,
        UserManager<AppUser> userManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<UserRegisterCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork,
        IGeocodingService geocodingService,
        IConfiguration configuration,
        IMailNotificationService mailNotificationService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _geocodingService = geocodingService;
        _configuration = configuration;
        _mailNotificationService = mailNotificationService;
    }

    public async Task<Result<UserAuthenticationResponse>> Handle(
        UserRegisterCommand request,
        CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Email == request.Email || u.UserName == request.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserDuplicate));
        }

        user = _mapper.Map<AppUser>(request);
        user.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        AddressesCustomer addressesCustomer = new AddressesCustomer
        {
            Id = Guid.NewGuid(),
            AppUserId = user.Id,
            IsPrimary = true,
            HouseNumber = request.HouseNumber,
            StreetName = request.StreetName,
            CreatedAt = _dateTimeProvider.VietnamDateTimeNow
        };

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }
        addressesCustomer.CityId = request.CityId;

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }
        addressesCustomer.DistrictId = request.DistrictId;

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }
        addressesCustomer.WardId = request.WardId;

        // convert to lat and long
        string fullAddress = $"{request.HouseNumber} {request.StreetName}, {ward.Name}, {district.Name}, {city.Name}";
        (double, double)? mostAccurateLocation = await _geocodingService.ConvertToLatLongAsync(fullAddress);

        if (mostAccurateLocation == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.LatLongNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.LatLongNotFound));
        }

        addressesCustomer.Latitude = (decimal)mostAccurateLocation.Value.Item1;
        addressesCustomer.Longitude = (decimal)mostAccurateLocation.Value.Item2;

        user.AddressesCustomers.Add(addressesCustomer);

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join("\n", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if (string.IsNullOrEmpty(request.Role))
        {
            await _userManager.AddToRoleAsync(user, RoleConstants.Customer);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        string baseUrl = _configuration["AppSettings:BaseUrl"];
        string confirmationLink = $"{baseUrl}/api/v1/user-auth/confirm-email?email={user.Email}&encodedToken={encodedToken}";

        await _mailNotificationService.SendEmailConfirmationAsync(user.UserName, user.Email, confirmationLink);

        return _mapper.Map<UserAuthenticationResponse>((user, encodedToken));
    }
}