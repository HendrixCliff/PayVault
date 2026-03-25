using MediatR;

namespace PayVault.Application.UseCases.Savings.Commands.DepositSavings
{
    public class DepositSavingsCommand : IRequest<DepositSavingsResponse>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }

    public class DepositSavingsResponse
    {
        public string PaymentReference { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }
}