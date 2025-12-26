using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Auth;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService,
    IUserService userService,
    IRefreshTokenRepository refreshTokenRepository,
    IMinioClientWrapper minioClient,
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
            var errors = result.Errors.Select(e => e.Description).ToList();
            return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Registration failed", new Dictionary<string, object> { { "errors", errors } });
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        await userManager.AddToRoleAsync(user, role);
        await EnsureDefaultRolesAsync();

        var authResponse = await IssueTokensAsync(user, ct);
        SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.ExpiresAt);
        SetAccessTokenCookie(authResponse.AccessToken, authResponse.ExpiresAt);
        return this.ApiOk(authResponse, "Registration successful");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.INVALID_CREDENTIALS, "Invalid email or password");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            return this.ApiUnauthorized(ErrorCodes.INVALID_CREDENTIALS, "Invalid email or password");
        }

        var authResponse = await IssueTokensAsync(user, ct);
        SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.ExpiresAt);
        SetAccessTokenCookie(authResponse.AccessToken, authResponse.ExpiresAt);
        return this.ApiOk(authResponse, "Login successful");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var existing = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        if (existing is null || existing.IsRevoked)
        {
            return this.ApiUnauthorized(ErrorCodes.TOKEN_INVALID, "Invalid refresh token");
        }

        if (existing.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await refreshTokenRepository.RevokeAsync(existing, ct);
            await refreshTokenRepository.SaveChangesAsync(ct);
            return this.ApiUnauthorized(ErrorCodes.TOKEN_EXPIRED, "Refresh token has expired");
        }

        var user = existing.User ?? await userManager.FindByIdAsync(existing.UserId.ToString());
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        await refreshTokenRepository.RevokeAsync(existing, ct);
        await refreshTokenRepository.SaveChangesAsync(ct);

        var authResponse = await IssueTokensAsync(user, ct);
        SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.ExpiresAt);
        SetAccessTokenCookie(authResponse.AccessToken, authResponse.ExpiresAt);
        return this.ApiOk(authResponse, "Token refreshed successfully");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        var dto = await userService.MapToDtoAsync(user, ct);
        return this.ApiOk(dto, "Profile retrieved successfully");
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public Task<IActionResult> ForgotPassword([FromBody] DTOs.Auth.ForgotPasswordRequest request, CancellationToken ct)
    {
        // Implementation would send password reset email
        logger.LogInformation("Password reset requested for email {Email}", request.Email);
        return Task.FromResult<IActionResult>(this.ApiOk("If the email exists, a password reset link has been sent."));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public Task<IActionResult> ResetPassword([FromBody] DTOs.Auth.ResetPasswordRequest request, CancellationToken ct)
    {
        // Implementation would validate token and reset password
        logger.LogInformation("Password reset attempted with token");
        return Task.FromResult<IActionResult>(this.ApiOk("Password has been reset successfully."));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] DTOs.Auth.ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Password change failed", new Dictionary<string, object> { { "errors", errors } });
        }

        logger.LogInformation("Password changed for user {UserId}", userId);
        return this.ApiOk("Password changed successfully");
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] DTOs.Auth.UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (request.Bio is not null) user.Bio = request.Bio;
        if (request.Phone is not null) user.Phone = request.Phone;
        if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
        if (request.TargetBandScore.HasValue) user.TargetBandScore = request.TargetBandScore;
        if (request.CurrentLevel is not null) user.CurrentLevel = request.CurrentLevel;
        if (request.ExamDate.HasValue) user.ExamDate = request.ExamDate;

        user.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Profile update failed", new Dictionary<string, object> { { "errors", errors } });
        }

        var dto = await userService.MapToDtoAsync(user, ct);
        return this.ApiOk(dto, "Profile updated successfully");
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        return await Me(ct);
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    [RequestSizeLimit(10_000_000)] // 10MB limit
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile avatar, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        if (avatar is null || avatar.Length == 0)
        {
            return this.ApiBadRequest(ErrorCodes.REQUIRED_FIELD_MISSING, "Avatar file is required");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(avatar.ContentType.ToLower()))
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "Invalid file type. Allowed types: JPEG, PNG, GIF, WEBP");
        }

        // Validate file size (max 10MB)
        if (avatar.Length > 10_000_000)
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "File size exceeds 10MB limit");
        }

        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(avatar.FileName);
            var objectName = $"avatar_{user.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";

            // Upload to MinIO
            string avatarUrl;
            using (var stream = avatar.OpenReadStream())
            {
                avatarUrl = await minioClient.UploadImageAsync(stream, objectName, user.Id, ct);
            }

            // Update user's avatar URL
            user.AvatarUrl = avatarUrl;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Avatar update failed", new Dictionary<string, object> { { "errors", errors } });
            }

            logger.LogInformation("User {UserId} uploaded avatar: {AvatarUrl}", userId, avatarUrl);
            
            var userDto = await userService.MapToDtoAsync(user, ct);
            return this.ApiOk(new { avatarUrl, user = userDto }, "Avatar uploaded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading avatar for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to upload avatar", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    [HttpDelete("avatar")]
    [Authorize]
    public async Task<IActionResult> DeleteAvatar(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        user.AvatarUrl = null;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Avatar deletion failed", new Dictionary<string, object> { { "errors", errors } });
        }

        logger.LogInformation("User {UserId} deleted avatar", userId);
        return this.ApiOk("Avatar deleted successfully");
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

        // Clear cookies
        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Delete("accessToken");

        logger.LogInformation("User logged out");
        return this.ApiOk("Logged out successfully");
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

    private void SetRefreshTokenCookie(string refreshToken, DateTimeOffset expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            // Using None because frontend (Next.js) runs on a different origin (localhost:3000)
            SameSite = SameSiteMode.None,
            Expires = expiresAt,
            Path = "/"
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private void SetAccessTokenCookie(string accessToken, DateTimeOffset expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = false, // cho phép Next.js đọc và gắn vào Authorization header nếu cần
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt,
            Path = "/"
        };

        Response.Cookies.Append("accessToken", accessToken, cookieOptions);
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

