namespace NairaLedger.Tests.Domain;

/// <summary>
/// Verifies the money value object invariants and arithmetic.
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsArgumentException()
    {
        Action act = () => new Money(-1);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*negative*")
            .And.ParamName.Should().Be("amount");
    }


    [Fact]
    public void Constructor_WithUnsupportedCurrency_ThrowsArgumentException()
    {
        Action act = () => new Money(100, "USD");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*NGN*");
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSucced()
    {
        var money = new Money(250.73m);

        money.Currency.Should().Be("NGN");
        money.Amount.Should().Be(250.73m);
    }

    [Fact]
    public void Addition_SameCurrency_ReturnsCorrectSum() 
    {
        // Arrange
        var money1 = new Money(100.34m);
        var money2 = new Money(200.66m);

        // Act
        var result = money1 + money2;
        
        // Assert
        result.Amount.Should().Be(301.00m);
        result.Currency.Should().Be("NGN");
    }

    [Fact]
    public void Substraction_WhenSufficient_ReturnsCorrectDifference()
    {
        // Arrange
        var money1 = new Money(500.00m);
        var money2 = new Money(150.25m);
        // Act
        var result = money1 - money2;
        // Assert
        result.Amount.Should().Be(349.75m);
        result.Currency.Should().Be("NGN");
    }

    [Fact]
    public void Substraction_WhenInsufficient_ThrowsInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100.00m);
        var money2 = new Money(150.25m);
        // Act
        Action act = () => _ = money1 - money2;
        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*insufficient*");
    }

    [Fact]
    public void Zero_ShouldHaveZeroAmount()
    {
        Money.Zero.Amount.Should().Be(0);
    }

    [Fact]
    public void Equality_TwoIdenticalAmounts_ShouldBeEqual()
    {
        var money1 = new Money(100.00m);
        var money2 = new Money(100.00m);
        money1.Should().Be(money2);
    }
}
