using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                Message = "Registratie mislukt.",
                Errors = createResult.Errors.Select(e => e.Description)
            });
        }

        // Publieke registratie kent altijd uitsluitend de rol Gast toe
        await _userManager.AddToRoleAsync(user, "Gast");

        return Ok(new { Message = "Registratie geslaagd. Rol 'Gast' is toegekend.", Email = user.Email });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new { Message = "Ongeldige inloggegevens." });
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

        return Ok(new
        {
            Message = "Succesvol ingelogd.",
            Email = dto.Email,
            Roles = roles
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Uitgelogd." });
    }
}