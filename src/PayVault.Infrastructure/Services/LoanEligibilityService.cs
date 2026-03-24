using PayVault.Domain.Entities;
using PayVault.Domain.Enums;
using PayVault.Infrastructure.Identity;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;

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

        public async Task<(bool IsEligible, string Reason)> IsEligibleAsync(ApplicationUser user, decimal amount)
        {
            // 1. Active loans check
            var userLoans = await _loanRepository.GetAllAsync();
            if (userLoans.Any(l => l.UserId == user.Id && l.Status == LoanStatus.Active))
                return (false, "User already has an active loan");

            // 2. Savings threshold
            var totalSavings = user.SavingsAccounts?.Sum(s => s.Balance) ?? 0m;
            if (totalSavings < amount * 0.3m)
                return (false, "Insufficient savings (minimum 30% required)");

            // 3. Loan limit
            if (amount > totalSavings * 2)
                return (false, "Loan exceeds maximum allowed limit");

            // 4. Risk score / external credit check
            int internalScore = CalculateRiskScore(user, userLoans);
            int externalScore = await _creditScoreService.GetScoreAsync(user);

            if (internalScore < 60 || externalScore < 60)
                return (false, $"Credit risk too high (internal: {internalScore}, external: {externalScore})");

            return (true, "Eligible");
        }

        private int CalculateRiskScore(ApplicationUser user, IEnumerable<Loan> loans)
        {
            int score = 0;

            var userLoans = loans.Where(l => l.UserId == user.Id);
            if (!userLoans.Any(l => l.Status == LoanStatus.Rejected)) score += 30;
            if (userLoans.Count(l => l.Status == LoanStatus.Completed) > 3) score += 30;

            var totalSavings = user.SavingsAccounts?.Sum(s => s.Balance) ?? 0m;
            if (totalSavings > 100_000) score += 20;
            if (!userLoans.Any(l => l.Status == LoanStatus.Active)) score += 20;

            return score;
        }
    }
}