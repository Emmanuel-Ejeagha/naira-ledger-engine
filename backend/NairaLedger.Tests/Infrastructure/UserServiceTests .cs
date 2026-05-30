namespace NairaLedger.Tests.Infrastructure;

public class UserServiceTests : IntegrationTestBase
{
    private IUserService UserService => ServiceProvider.GetRequiredService<IUserService>();

    [Fact]
    public async Task CreateUser_ThenFindByEmail_ShouldReturnUser()
    {
        var result = await UserService.CreateUserAsync("newuser@example.com", "New User", "Str0ngPass!", CancellationToken.None);
        result.UserId.Should().NotBeEmpty();

        var user = await UserService.FindByEmailAsync("newuser@example.com");
        user.Should().NotBeNull();
        user!.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task ValidatePassword_WithCorrectPassword_ShouldNotThrow()
    {
        await UserService.CreateUserAsync("pwtest@example.com", "Pw Test", "CorrectPass1!", CancellationToken.None);
        Func<Task> act = () => UserService.ValidatePasswordAsync("pwtest@example.com", "CorrectPass1!", CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidatePassword_WithWrongPassword_ShouldThrowUnauthorized()
    {
        await UserService.CreateUserAsync("wrong@example.com", "Wrong", "InitialPass1!", CancellationToken.None);
        Func<Task> act = () => UserService.ValidatePasswordAsync("wrong@example.com", "BadPass", CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetById_ShouldReturnUserDto()
    {
        var result = await UserService.CreateUserAsync("byid@example.com", "ById", "ByIdPass1!", CancellationToken.None);
        var user = await UserService.GetByIdAsync(result.UserId);
        user.Should().NotBeNull();
        user!.Email.Should().Be("byid@example.com");
    }
}