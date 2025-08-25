using EagleBank.API.Model;

namespace EagleBank.Api.Models;

public class Transaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; } 

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    //mavigation property
    public Account? Account { get; set; }
}
