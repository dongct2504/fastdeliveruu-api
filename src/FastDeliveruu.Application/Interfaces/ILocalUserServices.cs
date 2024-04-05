using FastDeliveruu.Application.Dtos.LocalUserDtos;

namespace FastDeliveruu.Application.Interfaces;

public interface ILocalUserServices
{
    Task<bool> IsUniqueUserAsync(string username);

    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);

    Task<LocalUserDto> RegisterAsync(RegisterationRequestDto registerationRequestDto);
}