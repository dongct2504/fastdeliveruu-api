using FastDeliveruu.Application.Dtos.LocalUserDtos;

namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateTokenAsync(LocalUserDto localUserDto);
}