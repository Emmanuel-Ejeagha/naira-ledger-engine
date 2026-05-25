namespace NairaLedger.Tests.Domain;

/// <summary>
/// Tests for the Wallet aggregate root: creation, KYC progression, activation.
/// </summary>
public class WalletTests
{
    [Fact]
    public void CreateWallet_ShouldInitializeWithUnverifiedKycAndRaiseEvent()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var wallet = new Wallet(userId, new WalletTag("Main"));

        // Act
        wallet.KycLevel.Should().Be(KycLevel.Unverified);
        wallet.IsActive.Should().BeTrue();
        wallet.Tag!.Value.Should().Be("Main");

        // Assert
        wallet.DomainEvents.Should().ContainSingle(e => e is WalletCreatedEvent);
        var createdEvent = wallet.DomainEvents.OfType<WalletCreatedEvent>().Single();
        createdEvent.UserId.Should().Be(userId);
    }

    [Fact]
    public void VerifyKyc_WhenUpgrading_ShouldSuccedAndRaiseEvent()
    {
        // Arrange
        var wallet = CreateSampleWallet();

        // Act
        wallet.VerifyKyc(KycLevel.Tier1);

        // Assert
        wallet.KycLevel.Should().Be(KycLevel.Tier1);
        wallet.DomainEvents.Should().Contain(e => e is KycVerifiedEvent);
    }

    [Fact]
    public void VerifyKyc_WhenDowngrading_ShouldThrow()
    {
        // Arrange
        var wallet = CreateSampleWallet();

        // Act
        wallet.VerifyKyc(KycLevel.Tier2);

        // Assert
        Action act = () => wallet.VerifyKyc(KycLevel.Tier1);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot downgrade KYC level from Tier2 to Tier1.");
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSetInActive()
    {
        // Arrange
        var wallet = CreateSampleWallet();

        // Act
        wallet.Deactivate();

        // Assert
        wallet.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_WhenInactive_ShouldSetActive()
    {
        // Arrange
        var wallet = CreateSampleWallet();

        // Act
        wallet.Deactivate();
        wallet.Activate();

        // Assert
        wallet.IsActive.Should().BeTrue();
    }

    private Wallet CreateSampleWallet() =>
        new Wallet(new UserId(Guid.NewGuid()));
}
