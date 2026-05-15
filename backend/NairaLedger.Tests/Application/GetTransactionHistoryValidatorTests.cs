using FluentAssertions;
using FluentValidation.TestHelper;
using NairaWallet.Application.Queries.GetTransactionHistory;

namespace NairaWallet.Tests.Application;

/// <summary>
/// Tests the GetTransactionHistoryValidator rules.
/// </summary>
public class GetTransactionHistoryValidatorTests
{
    private readonly GetTransactionHistoryValidator _validator = new();

    [Fact]
    public void Should_Allow_Valid_Query()
    {
        var query = new GetTransactionHistoryQuery(Guid.NewGuid(), "cursor", 25);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Allow_NullCursor()
    {
        var query = new GetTransactionHistoryQuery(Guid.NewGuid(), null, 50);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Reject_Empty_WalletId()
    {
        var query = new GetTransactionHistoryQuery(Guid.Empty, null, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.WalletId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Should_Reject_Invalid_PageSize(int invalidSize)
    {
        var query = new GetTransactionHistoryQuery(Guid.NewGuid(), null, invalidSize);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.PageSize);
    }
}