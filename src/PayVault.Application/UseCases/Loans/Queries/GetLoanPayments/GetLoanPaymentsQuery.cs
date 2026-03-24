using MediatR;
using PayVault.Application.DTOs.Loans;  


namespace PayVault.Application.UseCases.Loans.Queries.GetLoanPayments
{
    public class GetLoanPaymentsQuery : IRequest<List<LoanPaymentDto>>
{
    public Guid LoanId { get; set; }
}
}