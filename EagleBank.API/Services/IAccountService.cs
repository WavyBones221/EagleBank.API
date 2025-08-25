using EagleBank.Api.DTOs;

namespace EagleBank.Api.Services;

public interface IAccountService
{
    Task<AccountResponseDto> CreateAccountAsync(int userId, CreateAccountDto dto);
    Task<IEnumerable<AccountResponseDto>> GetAccountsAsync(int userId);
    Task<AccountResponseDto?> GetAccountByIdAsync(int accountId, int userId);
    Task<AccountResponseDto?> UpdateAccountAsync(int accountId, int userId, CreateAccountDto dto);
    Task<bool> DeleteAccountAsync(int accountId, int userId);
}
