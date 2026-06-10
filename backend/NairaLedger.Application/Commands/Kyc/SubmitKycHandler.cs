namespace NairaLedger.Application.Commands.Kyc;

public class SubmitKycHandler : IRequestHandler<SubmitKycCommand, Unit>
{
    private readonly IWalletRepository _walletRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitKycHandler(IWalletRepository walletRepo, IUnitOfWork unitOfWork)
    {
        _walletRepo = walletRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SubmitKycCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepo.GetByIdAsync(request.WalletId, cancellationToken);
        if (wallet is null) throw new InvalidOperationException("Wallet not found.");

        wallet.SubmitKyc(request.FullName, request.IdNumber, request.IdType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}