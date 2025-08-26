using EagleBank.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}
