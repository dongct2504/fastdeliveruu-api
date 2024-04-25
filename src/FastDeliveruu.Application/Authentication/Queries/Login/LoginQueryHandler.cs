using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;

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
        QueryOptions<LocalUser> options = new QueryOptions<LocalUser>
        {
            Where = u => u.UserName == request.UserName
        };
        LocalUser? localUser = await _localUserRepository.GetAsync(options);
        if (localUser == null)
        {
            return Result.Fail<AuthenticationResponse>(
                new NotFoundError("The user name is incorrect."));
        }

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, localUser.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationResponse>(
                new BadRequestError("The password is incorrect."));
        }

        bool isConfirmEmail = localUser.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            return Result.Fail<AuthenticationResponse>(
                new BadRequestError("The email is not yet confirmed."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(localUser.LocalUserId, localUser.Email,
            localUser.UserName, localUser.Role);

        return _mapper.Map<AuthenticationResponse>((localUser, token));
    }
}