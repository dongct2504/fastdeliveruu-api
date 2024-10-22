using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Identity.CustomManagers;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Commands.ShipperRegister;

public class ShipperRegisterCommandHandler : IRequestHandler<ShipperRegisterCommand, Result<ShipperAuthenticationResponse>>
{
    private readonly ShipperManager _shipperManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<ShipperRegisterCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public ShipperRegisterCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<ShipperRegisterCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper,
        ShipperManager shipperManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _shipperManager = shipperManager;
    }

    public async Task<Result<ShipperAuthenticationResponse>> Handle(ShipperRegisterCommand request, CancellationToken cancellationToken)
    {
        Shipper shipper = _mapper.Map<Shipper>(request);
        shipper.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }
        shipper.CityId = city.Id;

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }
        shipper.DistrictId = district.Id;

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }
        shipper.WardId = ward.Id;

        IdentityResult result = await _shipperManager.CreateAsync(shipper, request.Password);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join("\n", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string token = await _shipperManager.GenerateEmailConfirmationTokenAsync(shipper);

        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return _mapper.Map<ShipperAuthenticationResponse>((shipper, encodedToken));
    }
}
