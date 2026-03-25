using PayVault.Application.Common.Interfaces;

namespace PayVault.Application.Interfaces.Services
{
   public interface ILoanEligibilityService
{
    Task<(bool IsEligible, string Reason)> IsEligibleAsync(IApplicationUser user, decimal amount);
}
}