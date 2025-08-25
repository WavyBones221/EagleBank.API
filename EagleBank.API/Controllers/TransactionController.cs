using EagleBank.Api.DTOs;
using EagleBank.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EagleBank.Api.Controllers;

[ApiController]
[Route("v1/accounts/{accountId}/transactions")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(int accountId, [FromBody] CreateTransactionDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreateTransactionAsync(accountId, userId, dto);
            return CreatedAtAction(nameof(GetTransaction), new { accountId, transactionId = transaction.Id }, transaction);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(int accountId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.GetTransactionsAsync(accountId, userId);
            return Ok(transactions);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransaction(int accountId, int transactionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(accountId, transactionId, userId);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
