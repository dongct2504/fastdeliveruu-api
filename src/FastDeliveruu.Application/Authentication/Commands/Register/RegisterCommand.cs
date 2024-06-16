using FastDeliveruu.Application.Dtos.AppUserDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Commands.Register;

public class RegisterCommand : IRequest<Result<AuthenticationResponse>>
{
    public string UserName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? Role { get; set; }
}