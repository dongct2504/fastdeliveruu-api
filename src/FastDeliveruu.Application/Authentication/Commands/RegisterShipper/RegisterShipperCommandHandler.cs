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

namespace FastDeliveruu.Application.Authentication.Commands.RegisterShipper;

public class RegisterShipperCommandHandler : IRequestHandler<RegisterShipperCommand,
    Result<AuthenticationShipperResponse>>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RegisterShipperCommandHandler(
        IShipperRepository shipperRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _shipperRepository = shipperRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthenticationShipperResponse>> Handle(RegisterShipperCommand request,
        CancellationToken cancellationToken)
    {
        Shipper shipper = _mapper.Map<Shipper>(request);
        shipper.ShipperId = Guid.NewGuid();
        shipper.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        shipper.CreatedAt = DateTime.Now;
        shipper.UpdatedAt = DateTime.Now;

        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            Where = s => s.Cccd == shipper.Cccd
        };
        Shipper? isShipperExist = await _shipperRepository.GetAsync(options);
        if (isShipperExist != null)
        {
            string message = "The request shipper is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationShipperResponse>(new DuplicateError(message));
        }

        Shipper createdShipper = await _shipperRepository.AddAsync(shipper);

        shipper.ShipperId = createdShipper.ShipperId;

        string token = _jwtTokenGenerator.GenerateEmailConfirmationToken(shipper.ShipperId, shipper.Email);

        return _mapper.Map<AuthenticationShipperResponse>((shipper, token));
    }
}