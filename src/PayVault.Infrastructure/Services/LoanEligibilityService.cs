using PayVault.Application.Common.Interfaces;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;

namespace PayVault.Infrastructure.Services
{
    public class LoanEligibilityService : ILoanEligibilityService
    {
        private readonly IRepository<Loan> _loanRepository;
        private readonly ICreditScoreService _creditScoreService;

        public LoanEligibilityService(
            IRepository<Loan> loanRepository,
            ICreditScoreService creditScoreService)
        {
            _loanRepository = loanRepository;
            _creditScoreService = creditScoreService;
        }

        public async Task<(bool IsEligible, string Reason)> IsEligibleAsync(IApplicationUser user, decimal amount)
        {
          
            Guid userGuid = Guid.Parse(user.Id);

            var userLoans = (await _loanRepository.GetAllAsync())
                .Where(l => l.UserId == userGuid);

            if (userLoans.Any(l => l.Status == LoanStatus.Active))
                return (false, "User already has an active loan");

            decimal totalSavings = 0m; // TODO: replace with real savings data
            if (totalSavings < amount * 0.3m)
                return (false, "Insufficient savings (minimum 30% required)");

            if (amount > totalSavings * 2)
                return (false, "Loan exceeds maximum allowed limit");

            // Risk scoring (can be customized)
            int internalScore = CalculateRiskScore(userLoans, totalSavings);
            int externalScore = await _creditScoreService.GetScoreAsync(user.Id);

            if (internalScore < 60 || externalScore < 60)
                return (false, $"Credit risk too high (internal: {internalScore}, external: {externalScore})");

            return (true, "Eligible");
        }

        private int CalculateRiskScore(IEnumerable<Loan> loans, decimal totalSavings)
        {
            int score = 0;
            if (!loans.Any(l => l.Status == LoanStatus.Rejected)) score += 30;
            if (loans.Count(l => l.Status == LoanStatus.Completed) > 3) score += 30;
            if (totalSavings > 100_000) score += 20;
            if (!loans.Any(l => l.Status == LoanStatus.Active)) score += 20;
            return score;
        }
    }
}