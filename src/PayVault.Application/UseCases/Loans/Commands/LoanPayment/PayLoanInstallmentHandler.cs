using MediatR;
using PayVault.Application.UseCases.Loans.Commands.LoanPayment;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Domain.Entities;

namespace PayVault.Application.UseCases.Loans.Commands.LoanPayment
{
    public class PayLoanInstallmentHandler : IRequestHandler<PayLoanInstallmentCommand>
{
    private readonly IRepository<Loan> _repository;

    public PayLoanInstallmentHandler(IRepository<Loan> repository)
    {
        _repository = repository;
    }

    public async Task Handle(PayLoanInstallmentCommand request, CancellationToken cancellationToken)
{
    var loan = await _repository.GetByIdAsync(request.LoanId);

    if (loan is null)
        throw new InvalidOperationException("Loan not found");

    loan.AddPayment(request.Amount);

    await _repository.UpdateAsync(loan);
}
}
}