using EagleBank.Api.Data;
using EagleBank.Api.DTOs;
using EagleBank.Api.Models;
using EagleBank.API.Model;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Api.Services;

public class AccountService : IAccountService
{
    private readonly BankContext _context;

    public AccountService(BankContext context)
    {
        _context = context;
    }

    public async Task<AccountResponseDto> CreateAccountAsync(int userId, CreateAccountDto dto)
    {
        var account = new Account
        {
            UserId = userId,
            AccountNumber = Guid.NewGuid().ToString("N")[..10],
            Balance = 0,
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            AccountName = dto.AccountName,
            Balance = account.Balance
        };
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAccountsAsync(int userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => new AccountResponseDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                AccountName = "Account",  // could be extended with a field later
                Balance = a.Balance
            })
            .ToListAsync();
    }

    public async Task<AccountResponseDto?> GetAccountByIdAsync(int accountId, int userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null || account.UserId != userId)
            return null;

        return new AccountResponseDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            AccountName = "Account",
            Balance = account.Balance
        };
    }

    public async Task<AccountResponseDto?> UpdateAccountAsync(int accountId, int userId, CreateAccountDto dto)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null || account.UserId != userId)
            return null;

        // Currently only updating AccountName, but could extend this
        var updated = new AccountResponseDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            AccountName = dto.AccountName,
            Balance = account.Balance
        };

        return updated;
    }

    public async Task<bool> DeleteAccountAsync(int accountId, int userId)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null || account.UserId != userId)
            return false;

        // Example rule: Don’t delete if transactions exist
        if (account.Transactions.Any())
            throw new InvalidOperationException("Cannot delete account with transactions.");

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();

        return true;
    }
}
