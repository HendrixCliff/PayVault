using MediatR;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.DTOs.Loans;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;

namespace PayVault.Application.UseCases.Loans.Queries
{
    public class GetLoansByStatusQueryHandler : IRequestHandler<GetLoansByStatusQuery, List<LoanDto>>
    {
        private readonly IRepository<Loan> _loanRepository;

        public GetLoansByStatusQueryHandler(IRepository<Loan> loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<List<LoanDto>> Handle(GetLoansByStatusQuery request, CancellationToken cancellationToken)
        {
            var allLoans = await _loanRepository.GetAllAsync();
            
            IEnumerable<Loan> filteredLoans;
            
            if (request.UserId.HasValue)
            {
                // User: filter by UserId + Status
                filteredLoans = allLoans.Where(l => l.UserId == request.UserId.Value && l.Status == request.Status);
            }
            else
            {
                // Admin: ALL loans by Status only
                filteredLoans = allLoans.Where(l => l.Status == request.Status);
            }
            
            var loansList = filteredLoans.ToList();
            
            return loansList.Select(l => new LoanDto
            {
                Id = l.Id,
                UserId = l.UserId,
                Amount = l.Amount,
                InterestRate = l.InterestRate,
                Installments = l.Installments,
                Status = l.Status,
                CreatedAt = l.CreatedAt
            }).ToList();
        }
    }
}