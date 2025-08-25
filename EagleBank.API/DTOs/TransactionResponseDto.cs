using EagleBank.Api.Models;

namespace EagleBank.Api.DTOs;

public class TransactionResponseDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal? BalanceAfterTransaction { get; set; }
}
