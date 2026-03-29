using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Application.DTOs;
using TechStore.Application.Services;

namespace TechStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = UserService.GetUserIdFromClaims(User);
        if (userId is null) return Unauthorized();

        var result = await _userService.GetProfileAsync(userId.Value);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = UserService.GetUserIdFromClaims(User);
        if (userId is null) return Unauthorized();

        var result = await _userService.UpdateProfileAsync(userId.Value, request);
        if (result is null) return NotFound();
        return Ok(result);
    }
}
