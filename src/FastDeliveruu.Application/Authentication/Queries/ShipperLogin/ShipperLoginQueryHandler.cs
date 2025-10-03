using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.ShipperLogin;

public class ShipperLoginQueryHandler : IRequestHandler<ShipperLoginQuery, Result<ShipperAuthenticationResponse>>
{
    private readonly SignInManager<Shipper> _signInManager;
    private readonly UserManager<Shipper> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ShipperLoginQueryHandler> _logger;
    private readonly IMapper _mapper;

    public ShipperLoginQueryHandler(
        SignInManager<Shipper> signInManager,
        UserManager<Shipper> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        ILogger<ShipperLoginQueryHandler> logger,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<ShipperAuthenticationResponse>> Handle(ShipperLoginQuery request, CancellationToken cancellationToken)
    {
        Shipper? shipper = await _userManager.FindByNameAsync(request.UserName);
        if (shipper == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WrongUserName} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WrongUserName));
        }

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(shipper);
        if (!isEmailConfirmed)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.EmailYetConfirmed} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.EmailYetConfirmed));
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(shipper, request.Password, false);
        if (!signInResult.Succeeded)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WrongPassword} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WrongPassword));
        }

        string token = _jwtTokenGenerator.GenerateTokenForShipper(shipper);

        _logger.LogInformation($"Shipper {shipper.UserName} login at: {_dateTimeProvider.VietnamDateTimeNow:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<ShipperAuthenticationResponse>((shipper, token));
    }
}
