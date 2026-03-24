using MediatR;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Domain.Entities;

namespace PayVault.Application.UseCases.Savings.Commands.DepositSavings
{
  public class DepositSavingsHandler : IRequestHandler<DepositSavingsCommand, DepositSavingsResponse>
{
    private readonly ISavingsPaymentService _paymentService;

    public DepositSavingsHandler(ISavingsPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<DepositSavingsResponse> Handle(DepositSavingsCommand request, CancellationToken cancellationToken)
    {
       
        var (reference, paymentUrl) = await _paymentService.InitializeDepositAsync(
            await _paymentService.GetAccountByIdAsync(request.AccountId), 
            request.Amount
        );

        return new DepositSavingsResponse
        {
            PaymentReference = reference,
            PaymentUrl = paymentUrl
        };
    }
}
}