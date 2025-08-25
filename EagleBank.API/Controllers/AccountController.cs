using EagleBank.Api.DTOs;
using EagleBank.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EagleBank.Api.Controllers;

[ApiController]
[Route("v1/accounts")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        var userId = GetCurrentUserId();
        var account = await _accountService.CreateAccountAsync(userId, dto);
        return CreatedAtAction(nameof(GetAccount), new { accountId = account.Id }, account);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var userId = GetCurrentUserId();
        var accounts = await _accountService.GetAccountsAsync(userId);
        return Ok(accounts);
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccount(int accountId)
    {
        var userId = GetCurrentUserId();
        var account = await _accountService.GetAccountByIdAsync(accountId, userId);

        if (account == null)
        {
            return NotFound();
        }


        return Ok(account);
    }

    [HttpPatch("{accountId}")]
    public async Task<IActionResult> UpdateAccount(int accountId, [FromBody] CreateAccountDto dto)
    {
        var userId = GetCurrentUserId();
        var account = await _accountService.UpdateAccountAsync(accountId, userId, dto);

        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(int accountId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _accountService.DeleteAccountAsync(accountId, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
