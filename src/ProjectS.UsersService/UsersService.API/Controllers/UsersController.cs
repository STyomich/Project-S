using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTO.Users;
using UsersService.Application.Interfaces;

namespace UsersService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    private readonly IUsersService _usersService = usersService;

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user =  await _usersService.GetUserShortInfoAsync(userId, cancellationToken);
        return Ok(user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var token = await _usersService.LoginAsync(request.Email, request.Password, cancellationToken);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        await _usersService.RegisterUserAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPatch("update-username")]
    public async Task<IActionResult> UpdateUserNameAsync([FromBody] UpdateUserNameRequest request, CancellationToken cancellationToken)
    {
        await _usersService.UpdateUserNameAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPatch("update-password")]
    public async Task<IActionResult> UpdateUserPasswordAsync([FromBody] UpdateUserPasswordRequest request, CancellationToken cancellationToken)
    {
        await _usersService.UpdateUserPasswordAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPatch("update-email")]
    public async Task<IActionResult> UpdateUserEmailAsync([FromBody] UpdateUserEmailRequest request, CancellationToken cancellationToken)
    {
        await _usersService.UpdateUserEmailAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("deactivate/{userId:guid}")]
    public async Task<IActionResult> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _usersService.DeactivateUserAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpGet("exists-by-email")]
    [AllowAnonymous]
    public async Task<IActionResult> IsUserExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var exists = await _usersService.IsUserExistsByEmailAsync(email, cancellationToken);
        return Ok(exists);
    }
}
