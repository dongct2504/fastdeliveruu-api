using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FluentResults;
using GoogleMaps.LocationServices;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenCage.Geocode;
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
    private readonly IConfiguration _configuration;

    public UserRegisterCommandHandler(
        IMapper mapper,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<UserRegisterCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
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
        string fullAddress = $"{request.Address}, {ward.Name}, {district.Name}, {city.Name}";

        //GoogleLocationService locationService = new GoogleLocationService(apikey: _configuration["Google:ApiKey"]);
        //MapPoint point = locationService.GetLatLongFromAddress(fullAddress);

        //addressesCustomer.Latitude = (decimal?)point.Latitude;
        //addressesCustomer.Longitude = (decimal?)point.Longitude;

        Geocoder geocoder = new Geocoder(_configuration["OpenCage:ApiKey"]);
        GeocoderResponse geocoderResponse = await geocoder.GeocodeAsync(fullAddress);

        Location? mostAccurateLocation = null;

        foreach (Location? item in geocoderResponse.Results)
        {
            if (!string.IsNullOrEmpty(item.Components.Road))
            {
                mostAccurateLocation = item;
            }
        }

        if (mostAccurateLocation == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.LatLongNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.LatLongNotFound));
        }

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

        if (request.UserName == "admin" || 
            request.UserName == "admin1" || 
            request.UserName == "admin2" ||
            request.UserName == "admin3" ||
            request.UserName == "admin4")
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