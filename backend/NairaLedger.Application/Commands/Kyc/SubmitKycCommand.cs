namespace NairaLedger.Application.Commands.Kyc;

public record SubmitKycCommand(Guid WalletId, string FullName, string IdNumber, string IdType) : IRequest<Unit>;