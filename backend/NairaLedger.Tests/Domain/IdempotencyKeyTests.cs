namespace NairaLedger.Tests.Domain;

public class IdempotencyKeyTests
{
    [Theory]
    [InlineData("TXN-12345")]
    [InlineData("abcde-67890-fghij")]
    [InlineData("user_123@domain.com")]
    [InlineData("a")]
    public void CreateIdempotencyKey_ShouldSucceed(string keyValue)
    {
        // Act
        var key = new IdempotencyKey(keyValue);
        // Assert
        key.Value.Should().Be(keyValue);
        key.ToString().Should().Be(keyValue);
    }

    [Fact]
    public void Constructor_WithMaxLength128_CreateSuccessfully()
    {
        var maxLengthKey = new string('a', 128);

        var key = new IdempotencyKey(maxLengthKey);

        key.Value.Should().HaveLength(128);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithNullOrWhiteSpace_ThrowsArgumentException(string? invalidValue)
    {
        Action act = () => new IdempotencyKey(invalidValue!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be empty*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void Constructor_WithLengthGreaterThan128_ThrowsArgumentException()
    {
        var longKey = new string('a', 129);

        Action act = () => new IdempotencyKey(longKey);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 128 characters*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void ImplicitOperator_ConvertsToString()
    {
        var key = new IdempotencyKey("TXN-KEY-12345");
        string keyValue = key;
        keyValue.Should().Be("TXN-KEY-12345");
    }

    [Fact]
    public void ExplicitOperator_ConvertsFromString()
    {
        string keyValue = "TXN-KEY-12345";
        var key = (IdempotencyKey)keyValue;
        key.Value.Should().Be("TXN-KEY-12345");
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        var key1 = new IdempotencyKey("TXN-12345");
        var key2 = new IdempotencyKey("TXN-12345");

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var key = new IdempotencyKey("TXN-12345");

        key.ToString().Should().Be("TXN-12345");
    }
}