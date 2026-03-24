using MediatR;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.UseCases.Loans;
using PayVault.Application.DTOs.Loans;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;
using System.Linq;

namespace PayVault.Application.UseCases.Loans.Queries
{
    public class GetUserLoansQueryHandler : IRequestHandler<GetUserLoansQuery, List<LoanDto>>
    {
        private readonly IRepository<Loan> _loanRepository;

        public GetUserLoansQueryHandler(IRepository<Loan> loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<List<LoanDto>> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
        {
            var allLoans = await _loanRepository.GetAllAsync();
            
            IEnumerable<Loan> filteredLoans;
            
            if (request.UserId.HasValue)
            {
                filteredLoans = allLoans.Where(l => l.UserId == request.UserId.Value);
            }
            else
            {
                filteredLoans = allLoans;  // Admin sees ALL
            }
            
            var loansList = filteredLoans.ToList();  // ✅ .ToList() fixes IEnumerable
            
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