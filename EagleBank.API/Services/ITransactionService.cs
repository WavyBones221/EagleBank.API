using EagleBank.Api.DTOs;

namespace EagleBank.Api.Services;

public interface ITransactionService
{
    Task<TransactionResponseDto> CreateTransactionAsync(int accountId, int userId, CreateTransactionDto dto);
    Task<IEnumerable<TransactionResponseDto>> GetTransactionsAsync(int accountId, int userId);
    Task<TransactionResponseDto?> GetTransactionByIdAsync(int accountId, int transactionId, int userId);
}
