using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;
using NairaLedger.Tests.Infrastructure;
using NairaWallet.Application.Interfaces;
using System.Security.Claims;

namespace NairaLedger.Tests.Infrastructure;

public class JwtTokenAndRefreshTests : IntegrationTestBase
{
    private ITokenService TokenService => ServiceProvider.GetRequiredService<ITokenService>();
    private IRefreshTokenStore RefreshStore => ServiceProvider.GetRequiredService<IRefreshTokenStore>();

    [Fact]
    public void GenerateAccessToken_ThenValidate_ShouldReturnPrincipal()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        var token = TokenService.GenerateAccessToken(claims);
        var principal = TokenService.ValidateAccessToken(token);
        principal.Should().NotBeNull();
    }

    [Fact]
    public async Task StoreAndValidateRefreshToken_ShouldReturnUserId()
    {
        var token = TokenService.GenerateRefreshToken();
        var userId = Guid.NewGuid();
        await RefreshStore.StoreAsync(token, userId, DateTime.UtcNow.AddDays(7), CancellationToken.None);

        var validUserId = await RefreshStore.ValidateAsync(token, CancellationToken.None);
        validUserId.Should().Be(userId);
    }

    [Fact]
    public async Task RevokeRefreshToken_ShouldInvalidate()
    {
        var token = TokenService.GenerateRefreshToken();
        var userId = Guid.NewGuid();
        await RefreshStore.StoreAsync(token, userId, DateTime.UtcNow.AddDays(1), CancellationToken.None);
        await RefreshStore.RevokeAsync(token, CancellationToken.None);

        var validUserId = await RefreshStore.ValidateAsync(token, CancellationToken.None);
        validUserId.Should().BeNull();
    }
}