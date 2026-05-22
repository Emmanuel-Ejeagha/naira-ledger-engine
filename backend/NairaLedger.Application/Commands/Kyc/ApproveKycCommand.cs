using MediatR;
using NairaLedger.Domain.Enums;

namespace NairaLedger.Application.Commands.Kyc;

public record ApproveKycCommand(Guid WalletId, KycLevel NewLevel) : IRequest<Unit>;