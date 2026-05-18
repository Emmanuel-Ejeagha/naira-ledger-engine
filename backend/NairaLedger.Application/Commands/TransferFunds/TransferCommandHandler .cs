using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Entities;
using NairaLedger.Domain.Enums;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Application.Commands.TransferFunds;

/// <summary>
/// Handles P2P transfers with balance validation and double‑entry posting.
/// </summary>
public class TransferCommandHandler : IRequestHandler<TransferCommand, TransferResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILedgerQueryService _ledgerQueryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransferCommandHandler> _logger;

    public TransferCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        ILedgerQueryService ledgerQueryService,
        IUnitOfWork unitOfWork,
        ILogger<TransferCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _ledgerQueryService = ledgerQueryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TransferResponse> Handle(TransferCommand command, CancellationToken cancellationToken)
    {
        var fromWallet = await _walletRepository.GetByIdAsync(command.FromWalletId, cancellationToken);
        if (fromWallet is null || !fromWallet.IsActive)
            throw new InvalidOperationException("Sender wallet is invalid or inactive.");

        var toWallet = await _walletRepository.GetByIdAsync(command.ToWalletId, cancellationToken);
        if (toWallet is null || !toWallet.IsActive)
            throw new InvalidOperationException("Recipient wallet is invalid or inactive.");

        var balance = await _ledgerQueryService.GetBalanceAsync(command.FromWalletId, cancellationToken);
        if (balance < command.Amount)
            throw new InvalidOperationException("Insufficient funds.");

        var reference = TransactionReference.Generate();
        var entries = new List<LedgerEntry>
        {
            new(command.FromWalletId, command.Amount, LedgerEntryDirection.Debit, "Transfer: debit sender"),
            new(command.ToWalletId, command.Amount, LedgerEntryDirection.Credit, "Transfer: credit receiver")
        };

        var transaction = new Transaction(reference, TransactionType.Transfer, entries, null);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _transactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Transfer completed: {Amount} NGN from {From} to {To} (Ref: {Ref})",
                command.Amount, command.FromWalletId, command.ToWalletId, reference.Value);

            return new TransferResponse(transaction.Id, "Transfer completed successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}