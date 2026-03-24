using PayVault.Domain.Entities;

namespace PayVault.Application.Interfaces.Services
{
    public interface IPaymentService
{
    Task<string> DisburseLoanAsync(Loan loan, ApplicationUser user);
    Task HandleWebhookAsync(string reference, decimal amount);
}
}