using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaWallet.Application.Interfaces;

namespace NairaWallet.Application.Commands.ReverseTransaction;

/// <summary>
/// Reverses a completed transaction by creating a new balanced reversal transaction.
/// </summary>
public class ReverseTransactionCommandHandler : IRequestHandler<ReverseTransactionCommand, ReverseTransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReverseTransactionCommandHandler> _logger;

    public ReverseTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReverseTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReverseTransactionResponse> Handle(ReverseTransactionCommand request, CancellationToken cancellationToken)
    {
        var original = await _transactionRepository.GetByIdWithEntriesAsync(request.TransactionId, cancellationToken);
        if (original is null)
            throw new InvalidOperationException("Original transaction not found.");

        var reversal = Transaction.CreateReversal(original, request.InitiatedByUserId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _transactionRepository.AddAsync(reversal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Transaction {OriginalId} reversed. Reversal TxId: {ReversalId}",
                original.Id, reversal.Id);

            return new ReverseTransactionResponse(reversal.Id, "Transaction reversed successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}