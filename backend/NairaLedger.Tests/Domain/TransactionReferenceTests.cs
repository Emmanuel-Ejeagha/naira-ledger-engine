using FluentAssertions;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests.Domain;

public class TransactionReferenceTests
{
    [Fact]
    public void Constructor_WithInvalidLength_ThrowsArgumentException()
    {
        Action act = () => new TransactionReference("too-short");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*exactly 24*");
    }

    [Fact]
    public void Geneerate_ProducesValidReference()
    {
        var reference = TransactionReference.Generate();
        reference.Value.Should().HaveLength(24);
        reference.Value.Should().StartWith("NW-");
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        reference.Value.Should().Contain(today);
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        var ref1 = new TransactionReference("NW-20240601-1A2B3C4D5E6F");
        var ref2 = new TransactionReference("NW-20240601-1A2B3C4D5E6F");
        ref1.Should().Be(ref2);
    }

    [Fact]
    public void Equality_DifferentValue_ShouldNotBeEqual()
    {
        var ref1 = TransactionReference.Generate();
        var ref2 = TransactionReference.Generate();
        ref1.Should().NotBe(ref2);
    }
}