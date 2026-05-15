using FluentAssertions;
using Moq;
using NairaWallet.Application.DTOs;
using NairaWallet.Application.Interfaces;
using NairaWallet.Application.Queries.GetTransactionHistory;

namespace NairaWallet.Tests.Application;

/// <summary>
/// Tests for the transaction history query handler.
/// </summary>
public class GetTransactionHistoryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPaginatedResultsFromQueryService()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var expectedResponse = new PagedResponse<TransactionDto>(
            new List<TransactionDto>
            {
                new(Guid.NewGuid(), "NW-20250101-ABC", "Transfer", "Completed", 500, DateTime.UtcNow)
            },
            "next-cursor",
            true);

        var mockQueryService = new Mock<ITransactionQueryService>();
        mockQueryService.Setup(q => q.GetTransactionsAsync(walletId, "start", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var handler = new GetTransactionHistoryHandler(mockQueryService.Object);
        var query = new GetTransactionHistoryQuery(walletId, "start", 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.NextCursor.Should().Be("next-cursor");
        result.HasMore.Should().BeTrue();
    }
}