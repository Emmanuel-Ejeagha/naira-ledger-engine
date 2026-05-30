namespace NairaLedger.Application.Commands.Auth.Register;

/// <summary>
/// Creates a new user and automatically provisions a wallet.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserService _userService;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserService userService,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateUserAsync(request.Email, request.FullName, request.Password, cancellationToken);

        var wallet = new Wallet(new UserId(user.UserId), new WalletTag("Main"));
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _walletRepository.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return new RegisterUserResponse(user.UserId, wallet.Id, "User registered successfully.");
    }
}