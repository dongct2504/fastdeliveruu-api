using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.LocalUsers;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        ILocalUserRepository localUserRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
        _localUserRepository = localUserRepository;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var spec = new UserByUsernameOrEmailSpecification(request.UserName, request.Email);
        LocalUser? localUser = await _localUserRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (localUser != null)
        {
            string message = "The email or username is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationResponse>(new DuplicateError(message));
        }

        localUser = _mapper.Map<LocalUser>(request);
        localUser.LocalUserId = Guid.NewGuid();
        localUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        localUser.Role = request.Role ?? "Customer";
        localUser.CreatedAt = DateTime.Now;
        localUser.UpdatedAt = DateTime.Now;

        await _localUserRepository.AddAsync(localUser);

        string token = _jwtTokenGenerator.GenerateEmailConfirmationToken(localUser.LocalUserId, localUser.Email);

        return _mapper.Map<AuthenticationResponse>((localUser, token));
    }
}