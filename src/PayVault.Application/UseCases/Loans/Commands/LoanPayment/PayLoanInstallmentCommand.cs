using MediatR;

namespace PayVault.Application.UseCases.Loans.Commands.LoanPayment
{
    public class PayLoanInstallmentCommand : IRequest
{
    public Guid LoanId { get; set; }

    public decimal Amount { get; set; }
}
}