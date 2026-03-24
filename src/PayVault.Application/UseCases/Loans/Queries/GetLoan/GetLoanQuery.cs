using MediatR;
using PayVault.Application.UseCases.Loans.Queries.GetLoan;

namespace PayVault.Application.UseCases.Loans.Queries.GetLoan
{
    public class GetLoanQuery : IRequest<GetLoanResult>
    {
        public Guid LoanId { get; set; }
    }
}