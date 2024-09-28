using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private const string TokenSecret = "ForTheLoveOfGodStoreAndLoadThisSecurely";
    private readonly RoleManager<IdentityRole> _roleManager; 
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    public IdentityController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            ModelState.AddModelError("Password", "Passwords do not match.");
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Optionally, you can add roles here
            if (request.Roles.Any())
            {
                await _userManager.AddToRolesAsync(user, request.Roles);
            }
            return Ok(new { Message = "User registered successfully" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return BadRequest(ModelState);
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenGenerationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TokenSecret);
        
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        // Retrieve roles and add them as claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = "https://id.invop.com",
            Audience = "https://movies.invop.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return Ok(new { token = jwt });
    }
    
    [HttpPost("role")]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreationRequest request)
    {
        if (await _roleManager.RoleExistsAsync(request.RoleName))
        {
            return BadRequest(new { Message = "Role already exists" });
        }

        var role = new IdentityRole(request.RoleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Role created successfully" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return BadRequest(ModelState);
    }
}

// Models for user registration and token generation
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class TokenGenerationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RoleCreationRequest
{
    public string RoleName { get; set; }
}