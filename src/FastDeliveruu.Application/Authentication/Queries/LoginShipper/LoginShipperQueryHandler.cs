using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Authentication.Queries.LoginShipper;

public class LoginShipperQueryHandler : IRequestHandler<LoginShipperQuery, Result<AuthenticationShipperResponse>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginShipperQueryHandler(
        IShipperRepository shipperRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _shipperRepository = shipperRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthenticationShipperResponse>> Handle(LoginShipperQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            Where = u => u.UserName == request.UserName
        };
        Shipper? shipper = await _shipperRepository.GetAsync(options, asNoTracking: true);
        if (shipper == null)
        {
            string message = "The user name is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationShipperResponse>(new NotFoundError(message));
        }

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, shipper.PasswordHash);
        if (!verified)
        {
            string message = "The password is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationShipperResponse>(new BadRequestError(message));
        }

        bool isConfirmEmail = shipper.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            string message = "The email is not yet confirmed.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationShipperResponse>(new BadRequestError(message));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(shipper.ShipperId, shipper.Email,
            shipper.UserName, "Shipper");

        Log.Information($"Shipper login at: {DateTime.Now:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<AuthenticationShipperResponse>((shipper, token));
    }
}