using PayVault.Domain.Entities;

namespace PayVault.Application.Interfaces.Services
{
    public interface ISavingsPaymentService
    {
      Task<(string PaymentReference, string PaymentUrl)> InitializeDepositAsync(SavingsAccount account, decimal amount);
        Task HandleWebhookAsync(string reference, decimal amount);
    }
}