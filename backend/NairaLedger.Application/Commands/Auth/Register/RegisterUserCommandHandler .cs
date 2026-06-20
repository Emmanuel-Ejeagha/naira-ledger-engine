using NairaLedger.Application.Commands.Auth.VerifyEmail;

namespace NairaLedger.Application.Commands.Auth.Register;

/// <summary>
/// Creates a new user and automatically provisions a wallet.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserService _userService;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<RegisterUserCommandHandler> _logger;


    public RegisterUserCommandHandler(
        IUserService userService,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userService = userService;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
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

        var message = "User registered successfully. Please check your email to verify your account.";
        try
        {
            await _mediator.Send(new SendVerificationEmailCommand(request.Email), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send verification email to {Email}", request.Email);
            message = "User registered, but we could not send the verification email. You can request a new one after logging in.";
        }

        return new RegisterUserResponse(user.UserId, wallet.Id, message);
    }
}