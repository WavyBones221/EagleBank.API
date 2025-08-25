using EagleBank.API.DTOs;
using EagleBank.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EagleBank.Api.Controllers;

[ApiController]
[Route("v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserServices _userService;

    public UsersController(IUserServices userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        var user = await _userService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _userService.AuthenticateAsync(dto);
        return Ok(new { token });
    }

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUser(int userId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _userService.GetUserByIdAsync(userId, requestingUserId);
        if (user == null) return NotFound();

        return Ok(user);
    }
}
