using MediatR;

namespace PayVault.Application.UseCases.Loans.Commands.RequestLoan
{
    public class RequestLoanCommand : IRequest<LoanResult>
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Installments { get; set; }

        public string BankCode { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
    }
}