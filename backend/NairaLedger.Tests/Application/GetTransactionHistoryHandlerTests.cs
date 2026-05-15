using FluentAssertions;
using Moq;
using NairaWallet.Application.DTOs;
using NairaWallet.Application.Interfaces;
using NairaWallet.Application.Queries.GetTransactionHistory;

namespace NairaWallet.Tests.Application;

/// <summary>
/// Comprehensive tests for the GetTransactionHistory query handler,
/// covering normal retrieval, empty lists, first-page null cursor, and edge cases.
/// </summary>
public class GetTransactionHistoryHandlerTests
{
    private readonly Mock<ITransactionQueryService> _queryServiceMock = new();
    private readonly GetTransactionHistoryHandler _handler;

    public GetTransactionHistoryHandlerTests()
    {
        _handler = new GetTransactionHistoryHandler(_queryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseFromService()
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

        _queryServiceMock
            .Setup(s => s.GetTransactionsAsync(walletId, "start-cursor", 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var query = new GetTransactionHistoryQuery(walletId, "start-cursor", 20);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _queryServiceMock.Verify(
            s => s.GetTransactionsAsync(walletId, "start-cursor", 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCursorIsNull_ShouldPassNullToService()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var expected = new PagedResponse<TransactionDto>(Array.Empty<TransactionDto>(), null, false);

        _queryServiceMock
            .Setup(s => s.GetTransactionsAsync(walletId, null, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new GetTransactionHistoryQuery(walletId, null, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithEmptyList_ShouldReturnEmptyResponse()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var expected = new PagedResponse<TransactionDto>(Array.Empty<TransactionDto>(), null, false);

        _queryServiceMock
            .Setup(s => s.GetTransactionsAsync(walletId, null, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new GetTransactionHistoryQuery(walletId, null, 50);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.HasMore.Should().BeFalse();
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectPageSizeEvenIfDefault()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var expected = new PagedResponse<TransactionDto>(Array.Empty<TransactionDto>(), null, false);

        _queryServiceMock
            .Setup(s => s.GetTransactionsAsync(walletId, null, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new GetTransactionHistoryQuery(walletId, null); // page size defaults to 20

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _queryServiceMock.Verify(
            s => s.GetTransactionsAsync(walletId, null, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        _queryServiceMock
            .Setup(s => s.GetTransactionsAsync(walletId, null, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        var query = new GetTransactionHistoryQuery(walletId, null);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*DB failure*");
    }
}