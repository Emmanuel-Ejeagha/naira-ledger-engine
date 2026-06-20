using NairaWallet.Domain.DomainEvents;

namespace NairaLedger.Infrastructure.Services;

/// <summary>
/// Real fraud detection: velocity checks, automatic wallet freeze.
/// </summary>
public class FraudDetectionService : IFraudEscalationService
{
    private readonly NairaLedgerDbContext _context;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<FraudDetectionService> _logger;

    public FraudDetectionService(NairaLedgerDbContext context, IMediator mediator, IEmailService emailService, IWalletRepository walletRepository, ILogger<FraudDetectionService> logger)
    {
        _context = context;
        _mediator = mediator;
        _emailService = emailService;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task EscalateAsync(Guid walletId, string ruleName, string description, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Fraud rule {Rule} triggered for wallet {WalletId}: {Description}", ruleName, walletId, description);

        // Automatic freeze for critical rules
        if (ruleName == "HighVelocity" || ruleName == "SuspiciousPattern")
        {
            var wallet = await _context.Wallets.FindAsync(new object[] { walletId }, cancellationToken);
            if (wallet is not null && wallet.IsActive)
            {
                wallet.Deactivate();
                // After wallet.Deactivate(); and saving
                try
                {
                    var ownerInfo = await _walletRepository.GetOwnerInfoAsync(walletId, cancellationToken);
                    if (ownerInfo is not null)
                    {
                        await _emailService.SendWalletFrozenEmailAsync(ownerInfo.Email, ownerInfo.FullName, ruleName, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send frozen email for wallet {WalletId}", walletId);
                }
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogCritical("Wallet {WalletId} automatically frozen due to fraud rule {Rule}", walletId, ruleName);

                // Notify admin (via email, push, etc.) – for now we just log and raise another domain event
                await _mediator.Publish(new DomainEventNotification<WalletFrozenEvent>(
                    new WalletFrozenEvent(walletId, ruleName)), cancellationToken);
            }
        }
    }
}