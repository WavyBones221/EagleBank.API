using EagleBank.API.DTOs;

namespace EagleBank.API.Services
{
    public interface IUserServices
    {
        Task<UserResponseDto> RegisterAsync(CreateUserDto dto);
        Task<string> AuthenticateAsync(LoginRequestDto dto); //returning JWT token
        Task<UserResponseDto?> GetUserByIdAsync(int userId, int requestingUserId);
    }
}
