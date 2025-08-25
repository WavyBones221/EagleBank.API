using EagleBank.Api.Models;

namespace EagleBank.Api.DTOs;

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}
