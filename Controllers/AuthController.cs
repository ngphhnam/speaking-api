using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Auth;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService,
    IUserService userService,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<AuthController> logger) : ControllerBase
{
    private static readonly string[] DefaultRoles = ["Student", "Teacher", "Admin"];

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? "Student" : request.Role!;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        await userManager.AddToRoleAsync(user, role);
        await EnsureDefaultRolesAsync();

        var authResponse = await IssueTokensAsync(user, ct);
        return Ok(authResponse);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var authResponse = await IssueTokensAsync(user, ct);
        return Ok(authResponse);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var existing = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        if (existing is null || existing.IsRevoked)
        {
            return Unauthorized();
        }

        if (existing.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await refreshTokenRepository.RevokeAsync(existing, ct);
            await refreshTokenRepository.SaveChangesAsync(ct);
            return Unauthorized();
        }

        var user = existing.User ?? await userManager.FindByIdAsync(existing.UserId.ToString());
        if (user is null)
        {
            return Unauthorized();
        }

        await refreshTokenRepository.RevokeAsync(existing, ct);
        await refreshTokenRepository.SaveChangesAsync(ct);

        var authResponse = await IssueTokensAsync(user, ct);
        return Ok(authResponse);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Unauthorized();
        }

        var dto = await userService.MapToDtoAsync(user, ct);
        return Ok(dto);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public Task<IActionResult> ForgotPassword([FromBody] DTOs.Auth.ForgotPasswordRequest request, CancellationToken ct)
    {
        // Implementation would send password reset email
        logger.LogInformation("Password reset requested for email {Email}", request.Email);
        return Task.FromResult<IActionResult>(Ok(new { message = "If the email exists, a password reset link has been sent." }));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public Task<IActionResult> ResetPassword([FromBody] DTOs.Auth.ResetPasswordRequest request, CancellationToken ct)
    {
        // Implementation would validate token and reset password
        logger.LogInformation("Password reset attempted with token");
        return Task.FromResult<IActionResult>(Ok(new { message = "Password has been reset successfully." }));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] DTOs.Auth.ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Unauthorized();
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        logger.LogInformation("Password changed for user {UserId}", userId);
        return Ok(new { message = "Password changed successfully." });
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] DTOs.Auth.UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName;
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var dto = await userService.MapToDtoAsync(user, ct);
        return Ok(dto);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetProfile(CancellationToken ct)
    {
        return await Me(ct);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest? request, CancellationToken ct)
    {
        if (request is not null && !string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var existing = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
            if (existing is not null)
            {
                await refreshTokenRepository.RevokeAsync(existing, ct);
                await refreshTokenRepository.SaveChangesAsync(ct);
            }
        }

        logger.LogInformation("User logged out");
        return Ok(new { message = "Logged out successfully." });
    }

    private async Task<AuthResponse> IssueTokensAsync(ApplicationUser user, CancellationToken ct)
    {
        var (accessToken, expiresAt) = await tokenService.GenerateAccessTokenAsync(user, ct);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await refreshTokenRepository.SaveChangesAsync(ct);

        var userDto = await userService.MapToDtoAsync(user, ct);
        logger.LogInformation("Issued tokens for user {UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            User = userDto
        };
    }

    private async Task EnsureDefaultRolesAsync()
    {
        foreach (var role in DefaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}

