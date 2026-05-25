namespace NairaLedger.Application.Commands.Kyc;

public class ApproveKycHandler : IRequestHandler<ApproveKycCommand, Unit>
{
    private readonly IWalletRepository _walletRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveKycHandler(IWalletRepository walletRepo, IUnitOfWork unitOfWork)
    {
        _walletRepo = walletRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ApproveKycCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepo.GetByIdAsync(request.WalletId, cancellationToken);
        if (wallet is null) throw new InvalidOperationException("Wallet not found.");
        wallet.VerifyKyc(request.NewLevel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}