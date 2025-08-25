using EagleBank.Api.Data;
using EagleBank.Api.Services;
using EagleBank.API.DTOs;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace EagleBank.Tests;

public class UserServiceTests
{
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new BankContext(options);

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"Jwt:Key", "MySuperSecretKeyThatHasToBeLongerOrItThrowsAnErrorwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"},
            {"Jwt:Issuer", "EagleBank.API"},
            {"Jwt:Audience", "EagleBank.Client"}
        }).Build();

        var jwtService = new JwtService(config);

        _userService = new UserService(context, jwtService);
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User()
    {
        var dto = new CreateUserDto { FullName = "Alice", Email = "alice@test.com", PasswordHash = "worldsStrongestPassword" };

        var result = await _userService.RegisterAsync(dto);

        Assert.AreEqual("Alice", result.FullName);
        Assert.AreEqual("alice@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var dto = new CreateUserDto { FullName = "Bob", Email = "bob@test.com", PasswordHash = "worldsStrongestPassword" };
        await _userService.RegisterAsync(dto);

        var token = await _userService.AuthenticateAsync(new LoginRequestDto { Email = "bob@test.com", PasswordHash = "worldsStrongestPassword" });

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));
    }
}
