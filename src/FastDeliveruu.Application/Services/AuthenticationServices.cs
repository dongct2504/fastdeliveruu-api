using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly ILocalUserServices _localUserServices;
    private readonly IShipperService _shipperService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public AuthenticationServices(IJwtTokenGenerator jwtTokenGenerator,
        ILocalUserServices localUserServices,
        IMapper mapper,
        IShipperService shipperService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _localUserServices = localUserServices;
        _mapper = mapper;
        _shipperService = shipperService;
    }

    public async Task<Result<AuthenticationResult>> RegisterAsync(
        RegisterationRequestDto request)
    {
        LocalUser localUser = _mapper.Map<LocalUser>(request);
        localUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        localUser.CreatedAt = DateTime.Now;
        localUser.UpdatedAt = DateTime.Now;

        Result<Guid> createLocalUserResult = await _localUserServices.AddUserAsync(localUser);
        if (createLocalUserResult.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new DuplicateError(createLocalUserResult.Errors[0].Message));
        }

        localUser.LocalUserId = createLocalUserResult.Value;

        string token = _jwtTokenGenerator.GenerateEmailConfirmationToken(localUser.LocalUserId, localUser.Email);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = _mapper.Map<LocalUserDto>(localUser),
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<AuthenticationShipperResult>> RegisterShipperAsync(RegisterationShipperDto
        request)
    {
        Shipper shipper = _mapper.Map<Shipper>(request);
        shipper.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        shipper.CreatedAt = DateTime.Now;
        shipper.UpdatedAt = DateTime.Now;

        Result<Guid> createShipperResult = await _shipperService.CreateShipperAsync(shipper);
        if (createShipperResult.IsFailed)
        {
            return Result.Fail<AuthenticationShipperResult>(new DuplicateError(
                createShipperResult.Errors[0].Message));
        }

        shipper.ShipperId = createShipperResult.Value;

        string token = _jwtTokenGenerator.GenerateEmailConfirmationToken(shipper.ShipperId, shipper.Email);

        AuthenticationShipperResult authenticationShipperResult = new AuthenticationShipperResult
        {
            ShipperDto = _mapper.Map<ShipperDto>(shipper),
            Token = token
        };

        return authenticationShipperResult;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        Result<LocalUser> localUserResult =
            await _localUserServices.GetLocalUserByUserNameAsync(loginRequestDto.UserName);
        if (localUserResult.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The username is incorrect."));
        }

        LocalUser localUser = localUserResult.Value;

        bool verified = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, localUser.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The password is incorrect."));
        }

        bool isConfirmEmail = localUser.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The email is not yet confirmed."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(localUser.LocalUserId, localUser.Email,
            localUser.UserName, localUser.Role);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = _mapper.Map<LocalUserDto>(localUser),
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<AuthenticationShipperResult>> LoginShipperAsync(LoginShipperDto request)
    {
        Result<Shipper> getShipperResult =
            await _shipperService.GetShipperByUserNameAsync(request.UserName);
        if (getShipperResult.IsFailed)
        {
            return Result.Fail<AuthenticationShipperResult>(
                new BadRequestError("The username is incorrect."));
        }

        Shipper shipper = getShipperResult.Value;

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, shipper.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationShipperResult>(
                new BadRequestError("The password is incorrect."));
        }

        bool isConfirmEmail = shipper.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            return Result.Fail<AuthenticationShipperResult>(
                new BadRequestError("The email is not yet confirmed."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(shipper.ShipperId, shipper.Email,
            shipper.UserName, "Shipper");

        AuthenticationShipperResult authenticationResult = new AuthenticationShipperResult
        {
            ShipperDto = _mapper.Map<ShipperDto>(shipper),
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<bool>> IsEmailConfirm(string token, string email)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        // Retrieve the user ID and email from the token
        Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        Claim? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

        if (userIdClaim == null || emailClaim == null)
        {
            return Result.Fail<bool>(new BadRequestError("Invalid token format."));
        }

        // Retrieve the user based on email
        Result<LocalUser> getLocalUserResult =
            await _localUserServices.GetLocalUserByEmailAsync(email);

        if (getLocalUserResult.IsSuccess)
        {
            // Verify the email confirmation token
            if (emailClaim.Value != email)
            {
                return Result.Fail<bool>(new BadRequestError("Email not match."));
            }

            // Update the user's email confirmation status
            getLocalUserResult.Value.IsConfirmEmail = true;
            getLocalUserResult.Value.UpdatedAt = DateTime.Now;

            await _localUserServices.UpdateUserAsync(Guid.Parse(userIdClaim.Value), getLocalUserResult.Value);
        }

        Result<Shipper> getShipperResult =
            await _shipperService.GetShipperByEmailAsync(email);

        if (getShipperResult.IsSuccess)
        {
            // Verify the email confirmation token
            if (emailClaim.Value != email)
            {
                return Result.Fail<bool>(new BadRequestError("Email not match."));
            }

            // Update the user's email confirmation status
            getShipperResult.Value.IsConfirmEmail = true;
            getShipperResult.Value.UpdatedAt = DateTime.Now;

            await _shipperService.UpdateShipperAsync(Guid.Parse(userIdClaim.Value), getShipperResult.Value);
        }

        if (getLocalUserResult.IsFailed && getShipperResult.IsFailed)
        {
            return Result.Fail<bool>(new NotFoundError("User not found."));
        }

        return true;
    }
}