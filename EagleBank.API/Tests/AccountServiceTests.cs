using EagleBank.Api.Data;
using EagleBank.Api.DTOs;
using EagleBank.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace EagleBank.Tests;

public class AccountServiceTests
{
    private readonly AccountService _accountService;
    private readonly BankContext _context;

    public AccountServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BankContext(options);
        _accountService = new AccountService(_context);
    }

    [Fact]
    public async Task CreateAccountAsync_Should_Create_Account()
    {
        var dto = new CreateAccountDto { AccountName = "Checking" };

        var account = await _accountService.CreateAccountAsync(1, dto);

        Assert.IsNotNull(account);
        Assert.AreEqual("Checking", account.AccountName);
        Assert.AreEqual(0, account.Balance);
    }

    [Fact]
    public async Task GetAccountsAsync_Should_Return_User_Accounts()
    {
        var dto = new CreateAccountDto { AccountName = "Savings" };
        await _accountService.CreateAccountAsync(1, dto);

        var accounts = await _accountService.GetAccountsAsync(1);

        Assert.ContainsSingle(accounts);
    }
}
