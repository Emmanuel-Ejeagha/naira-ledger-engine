using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;

namespace NairaLedger.Tests.Infrastructure;

public class EmailServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task SendCreditAlert_WhenSmtpUnavailable_ShouldNotThrow()
    {
        var emailService = ServiceProvider.GetRequiredService<IEmailService>();
        Func<Task> act = () => emailService.SendCreditAlertAsync("test@example.com", "Test", 100, "ref", CancellationToken.None);
        await act.Should().NotThrowAsync();
    }
}