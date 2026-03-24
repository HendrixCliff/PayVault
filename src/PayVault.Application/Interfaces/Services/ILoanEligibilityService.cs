

namespace PayVault.Application.Interfaces.Services {
    public interface ILoanEligibilityService
{
    Task<(bool IsEligible, string Reason)> CheckEligibilityAsync(ApplicationUser user, Loan loan);
}
}