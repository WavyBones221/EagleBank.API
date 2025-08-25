using EagleBank.Api.Data;
using EagleBank.Api.DTOs;
using EagleBank.Api.Models;
using EagleBank.Api.Services;
using EagleBank.API.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace EagleBank.Tests;

public class TransactionServiceTests
{
    private readonly TransactionService _transactionService;
    private readonly BankContext _context;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BankContext(options);

        var user = new User { Id = 1, FullName = "Charlie", Email = "charlie@test.com", PasswordHash = "worldsStrongestPassword" };
        _context.Users.Add(user);
        var account = new Account { Id = 1, UserId = 1, Balance = 100 };
        _context.Accounts.Add(account);
        _context.SaveChanges();

        _transactionService = new TransactionService(_context);
    }

    [Fact]
    public async Task Deposit_Should_Increase_Balance()
    {
        var dto = new CreateTransactionDto { Amount = 50, Type = TransactionType.Deposit };

        var result = await _transactionService.CreateTransactionAsync(1, 1, dto);

        Assert.AreEqual(TransactionType.Deposit, result.Type);
        Assert.AreEqual(150, result.BalanceAfterTransaction);
    }

    [Fact]
    public async Task Withdrawal_Should_Decrease_Balance()
    {
        var dto = new CreateTransactionDto { Amount = 40, Type = TransactionType.Withdrawal };

        var result = await _transactionService.CreateTransactionAsync(1, 1, dto);

        Assert.AreEqual(TransactionType.Withdrawal, result.Type);
        Assert.AreEqual(60, result.BalanceAfterTransaction);
    }

    [Fact]
    public async Task Withdrawal_Should_Fail_When_Insufficient_Funds()
    {
        var dto = new CreateTransactionDto { Amount = 200, Type = TransactionType.Withdrawal };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _transactionService.CreateTransactionAsync(1, 1, dto));
    }
}
