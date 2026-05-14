using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;
using NairaWallet.Application.Interfaces;

namespace NairaLedger.Application.Commands.CreateWallet;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, CreateWalletResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWalletCommandHandler> _logger;

    public CreateWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<CreateWalletCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateWalletResponse> Handle(CreateWalletCommand command, CancellationToken cancellationToken)
    {
        var existingWallet = await _walletRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existingWallet is not null)
        {
            _logger.LogWarning("User {UserId} already has a wallet with ID {WalletId}", command.UserId, existingWallet.Id);
            return new CreateWalletResponse(existingWallet.Id, "A wallet already exists for this user.");
        }

        var tag = command.Tag is not null ? new WalletTag(command.Tag) : null;
        var wallet = new Wallet(command.UserId, tag);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _walletRepository.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Created wallet {WalletId} for user {UserId}", wallet.Id, command.UserId.Value);
            return new CreateWalletResponse(wallet.Id, "Wallet created successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}