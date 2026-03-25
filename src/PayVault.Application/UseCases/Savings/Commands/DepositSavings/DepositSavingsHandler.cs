using MediatR;
using PayVault.Application.Interfaces.Services;
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
           
            SavingsAccount account = await _paymentService.GetAccountByIdAsync(request.AccountId);

            
            (string reference, string paymentUrl) = await _paymentService.InitializeDepositAsync(account, request.Amount);

            return new DepositSavingsResponse
            {
                PaymentReference = reference,
                PaymentUrl = paymentUrl
            };
        }
    }
}