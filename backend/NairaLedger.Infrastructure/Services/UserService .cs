using Microsoft.AspNetCore.Identity;
using NairaLedger.Application.Interfaces;
using NairaLedger.Infrastructure.Identity;

namespace NairaLedger.Infrastructure.Services;

/// <summary>
/// Adapter that wraps ASP.NET Identity UserManager into our application abstraction.
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) 
    {    
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<CreateUserResult> CreateUserAsync(string email, string fullName, string password, CancellationToken cancellationToken = default)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = false,
        };
        
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"user creation failed: {errors}");
        }

        return new CreateUserResult(user.Id, user.Email, user.FullName);
    }

    public async Task<UserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(email);
        return user is not null ? new UserDto(user.Id, user.Email!, user.FullName) : null;
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is not null ? new UserDto(user.Id, user.Email!, user.FullName) : null;
    }

    public async Task ValidatePasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid email or password.");
    }
    public async Task VerifyEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) throw new InvalidOperationException("User not found.");
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Email verification failed: {errors}");
        }
    }
}
