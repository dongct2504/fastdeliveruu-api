using FastDeliveruu.Application.Dtos.AppUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Commands.UserRegister;

public class UserRegisterCommand : IRequest<Result<UserAuthenticationResponse>>
{
    public string UserName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public string? Role { get; set; }
}