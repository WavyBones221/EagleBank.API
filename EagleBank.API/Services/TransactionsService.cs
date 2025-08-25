using EagleBank.Api.Data;
using EagleBank.Api.DTOs;
using EagleBank.Api.Models;
using Microsoft.EntityFrameworkCore;
using Transaction = EagleBank.Api.Models.Transaction;

namespace EagleBank.Api.Services;

public class TransactionService : ITransactionService
{
    private readonly BankContext _context;

    public TransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<TransactionResponseDto> CreateTransactionAsync(int accountId, int userId, CreateTransactionDto dto)
    {
        var account = await _context.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException("Forbidden: cannot access another user's account.");
        }

        if (dto.Type == TransactionType.Withdrawal && account.Balance < dto.Amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        var transaction = new Transaction
        {
            AccountId = accountId,
            Amount = dto.Amount,
            Type = dto.Type
        };

        // Update balance
        if (dto.Type == TransactionType.Deposit)
        {
            account.Balance += dto.Amount;
        }
        else if (dto.Type == TransactionType.Withdrawal)
        {
            account.Balance -= dto.Amount;
        }

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            AccountId = account.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Timestamp = transaction.Timestamp,
            BalanceAfterTransaction = account.Balance
        };
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsAsync(int accountId, int userId)
    {
        var account = await _context.Accounts.FindAsync(accountId);

        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException("Forbidden: cannot access another user's account.");
        }

        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Timestamp = t.Timestamp
            })
            .ToListAsync();
    }

    public async Task<TransactionResponseDto?> GetTransactionByIdAsync(int accountId, int transactionId, int userId)
    {
        var account = await _context.Accounts.FindAsync(accountId);

        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException("Forbidden: cannot access another user's account.");
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.AccountId == accountId);

        if (transaction == null)
        {
            return null;
        }


        return new TransactionResponseDto
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Timestamp = transaction.Timestamp
        };
    }
}
