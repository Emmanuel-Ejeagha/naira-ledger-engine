using NairaWallet.Domain.DomainEvents;

namespace NairaLedger.Infrastructure.Services;

/// <summary>
/// Real fraud detection: velocity checks, automatic wallet freeze.
/// </summary>
public class FraudDetectionService : IFraudEscalationService
{
    private readonly NairaLedgerDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<FraudDetectionService> _logger;

    public FraudDetectionService(NairaLedgerDbContext context, IMediator mediator, ILogger<FraudDetectionService> logger)
    {
        _context = context;
        _mediator = mediator;
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