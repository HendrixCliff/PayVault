using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.DTOs.Loans;
using PayVault.Application.UseCases.Loans.Queries.GetLoanPayments;  
using MediatR;
using PayVault.Domain.Entities;

namespace PayVault.Application.UseCases.Loans.Queries.GetLoanPayments
{
  public class GetLoanPaymentsHandler : IRequestHandler<GetLoanPaymentsQuery, List<LoanPaymentDto>>
{
    private readonly IRepository<Loan> _repository;


    public GetLoanPaymentsHandler(IRepository<Loan> repository)
    {
        _repository = repository;
    }

  public async Task<List<LoanPaymentDto>> Handle(GetLoanPaymentsQuery request, CancellationToken ct)
{
    var loan = await _repository.GetByIdAsync(request.LoanId);

    if (loan is null)
        throw new InvalidOperationException("Loan not found");

    return loan.Payments.Select(p => new LoanPaymentDto
    {
        Id = p.Id,
        Amount = p.Amount,
        Date = p.CreatedAt,
        RemainingBalance = loan.RemainingBalance ?? 0
    }).ToList();
}
}

}