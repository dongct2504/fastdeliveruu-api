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

namespace FastDeliveruu.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthenticationResponse>>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginQueryHandler(
        ILocalUserRepository localUserRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _localUserRepository = localUserRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthenticationResponse>> Handle(LoginQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new UserByUsernameSpecification(request.UserName);
        LocalUser? localUser = await _localUserRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (localUser == null)
        {
            string message = "The user name is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationResponse>(new BadRequestError(message));
        }

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, localUser.PasswordHash);
        if (!verified)
        {
            string message = "The password is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationResponse>(new BadRequestError(message));
        }

        bool isConfirmEmail = localUser.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            string message = "The email is not yet confirmed.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<AuthenticationResponse>(new BadRequestError(message));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(localUser.LocalUserId, localUser.Email,
            localUser.UserName, localUser.Role);

        Log.Information($"User login at: {DateTime.Now:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<AuthenticationResponse>((localUser, token));
    }
}