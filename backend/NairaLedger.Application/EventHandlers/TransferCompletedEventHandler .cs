namespace NairaLedger.Application.EventHandlers;

public class TransferCompletedEventHandler : INotificationHandler<DomainEventNotification<TransferCompletedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<TransferCompletedEventHandler> _logger;

    public TransferCompletedEventHandler(
        INotificationService notificationService,
        IEmailService emailService,
        IWalletRepository walletRepository,
        ILogger<TransferCompletedEventHandler> logger)
    {
        _notificationService = notificationService;
        _emailService = emailService;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<TransferCompletedEvent> wrapper, CancellationToken cancellationToken)
    {
        var domainEvent = wrapper.DomainEvent;
        _logger.LogInformation("Transfer completed: {From} -> {To}, Amount: {Amount}",
            domainEvent.FromWalletId, domainEvent.ToWalletId, domainEvent.Amount);

        var fromWallet = await _walletRepository.GetByIdAsync(domainEvent.FromWalletId, cancellationToken);
        var toWallet = await _walletRepository.GetByIdAsync(domainEvent.ToWalletId, cancellationToken);

        if (fromWallet is not null)
        {
            await _notificationService.SendToUserAsync(fromWallet.UserId.Value, "Transfer sent.", cancellationToken);
        }

        if (toWallet is not null)
        {
            await _notificationService.SendToUserAsync(toWallet.UserId.Value, "Transfer received.", cancellationToken);
        }

        var sender = await _walletRepository.GetOwnerInfoAsync(domainEvent.FromWalletId, cancellationToken);
        var receiver = await _walletRepository.GetOwnerInfoAsync(domainEvent.ToWalletId, cancellationToken);

        if (sender is not null)
            await _emailService.SendDebitAlertAsync(sender.Email, sender.FullName, domainEvent.Amount, domainEvent.TransactionId.ToString(), cancellationToken);
        if (receiver is not null)
            await _emailService.SendCreditAlertAsync(receiver.Email, receiver.FullName, domainEvent.Amount, domainEvent.TransactionId.ToString(), cancellationToken);
    }
}