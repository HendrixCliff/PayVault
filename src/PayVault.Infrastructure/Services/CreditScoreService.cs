using PayVault.Application.Interfaces.Services;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;

namespace PayVault.Infrastructure.Services
{
    public class CreditScoreService : ICreditScoreService
    {
        private readonly IRepository<Loan> _loanRepository;

        public CreditScoreService(IRepository<Loan> loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<int> GetScoreAsync(string userId)
        {
          
            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid userId");

           
            int score = 50;

            
            var loans = (await _loanRepository.GetAllAsync())
                        .Where(l => l.UserId == userGuid)
                        .ToList();

            if (!loans.Any())
            {
               
                score += 10;
            }
            else
            {
                foreach (var loan in loans)
                {
                    switch (loan.Status)
                    {
                        case LoanStatus.Completed:
                            score += 20; // good repayment history
                            break;
                        case LoanStatus.Active:
                            score += 10; // current good standing
                            break;
                        case LoanStatus.Rejected:
                        case LoanStatus.Defaulted:
                            score -= 20; // penalize for bad history
                            break;
                    }
                }
            }

            // Ensure score is between 0 and 100
            return Math.Clamp(score, 0, 100);
        }
    }
}