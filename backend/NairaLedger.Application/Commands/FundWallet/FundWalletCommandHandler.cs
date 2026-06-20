namespace NairaLedger.Application.Commands.FundWallet;

/// <summary>
/// Handles wallet funding by creating a balanced double‑entry transaction.
/// </summary>
public class FundWalletCommandHandler : IRequestHandler<FundWalletCommand, FundWalletResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ILogger<FundWalletCommandHandler> _logger;

    public FundWalletCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IEmailService emailService,
        ILogger<FundWalletCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<FundWalletResponse> Handle(FundWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(request.WalletId, cancellationToken);
        if (wallet is null)
            throw new InvalidOperationException($"Wallet {request.WalletId} not found.");
        if (!wallet.IsActive)
            throw new InvalidOperationException("Cannot fund an inactive wallet.");

        var bankWalletId = new Guid("00000000-0000-0000-0000-000000000001");

        var reference = TransactionReference.Generate();

        var entries = new List<LedgerEntry>
        {
            new(bankWalletId, request.Amount, LedgerEntryDirection.Debit, "Funding: debit bank float"),
            new(request.WalletId, request.Amount, LedgerEntryDirection.Credit, "Funding: credit user wallet")
        };

        var transaction = new Transaction(reference, TransactionType.Funding, entries, null);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _transactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            // In the Handle method, after CommitTransactionAsync:
            try
            {                
                var ownerInfo = await _walletRepository.GetOwnerInfoAsync(wallet.Id, cancellationToken);
                if (ownerInfo is not null)
                {
                    await _emailService.SendWalletFundedEmailAsync(ownerInfo.Email, ownerInfo.FullName, request.Amount, cancellationToken);
                }               
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send funding email for wallet {WalletId}", request.WalletId);
            }
            await _notificationService.SendToUserAsync(wallet.UserId.Value, $"Your wallet has been credited with NGN {request.Amount:N2}.", cancellationToken);

            _logger.LogInformation(
                "Funded wallet {WalletId} with {Amount} NGN (TxRef: {Ref})",
                request.WalletId, request.Amount, reference.Value);

            // Balance is derived later via query. We return 0 for now; the client queries separately.
            return new FundWalletResponse(transaction.Id, 0, "Wallet funded successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}