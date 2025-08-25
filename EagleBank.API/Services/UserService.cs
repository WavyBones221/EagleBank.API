using EagleBank.Api.Data;
using EagleBank.API.DTOs;
using EagleBank.API.Model;
using EagleBank.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EagleBank.Api.Services;

public class UserService : IUserServices
{
    private readonly BankContext _context;
    private readonly IJwtService _jwtService;

    public UserService(BankContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<UserResponseDto> RegisterAsync(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("Email already registered.");
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.PasswordHash)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponseDto { Id = user.Id, FullName = user.FullName, Email = user.Email };
    }

    public async Task<string> AuthenticateAsync(LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || user.PasswordHash != HashPassword(dto.PasswordHash)) 
        { 
            throw new UnauthorizedAccessException("Invalid credentials."); 
        }

        return _jwtService.GenerateToken(user);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int userId, int requestingUserId)
    {
        if (userId != requestingUserId)
        {
            throw new UnauthorizedAccessException("Forbidden access.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null) 
        { 
            return null; 
        
        }

        return new UserResponseDto { Id = user.Id, FullName = user.FullName, Email = user.Email };
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
