using EagleBank.Api.Models;

namespace EagleBank.API.Model
{
    public class Account
    {
        public int Id { get; set; }

        //"N" Refers to a string without hyphens, GUID collision is highly unlikely, In real banking systems you would need a proper account number generator but this is a coding test :)
        public string AccountNumber { get; set; } = Guid.NewGuid().ToString("N")[..10];
        public decimal Balance { get; set; } = 0;
        public int UserId { get; set; }

        //navigation
        public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
