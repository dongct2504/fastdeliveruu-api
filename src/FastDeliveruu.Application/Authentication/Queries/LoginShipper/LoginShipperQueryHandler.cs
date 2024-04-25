using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;

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
        Shipper? shipper = await _shipperRepository.GetAsync(options);
        if (shipper == null)
        {
            return Result.Fail<AuthenticationShipperResponse>(
                new NotFoundError("The user name is incorrect."));
        }

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, shipper.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationShipperResponse>(
                new BadRequestError("The password is incorrect."));
        }

        bool isConfirmEmail = shipper.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            return Result.Fail<AuthenticationShipperResponse>(
                new BadRequestError("The email is not yet confirmed."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(shipper.ShipperId, shipper.Email,
            shipper.UserName, "Shipper");

        return _mapper.Map<AuthenticationShipperResponse>((shipper, token));
    }
}