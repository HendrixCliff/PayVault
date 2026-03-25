using PayVault.Domain.Entities;
using PayVault.Application.DTOs.Auth;

namespace PayVault.Application.Interfaces.Services
{
    public interface IPaymentService
{
    Task<string> DisburseLoanAsync(Loan loan, ApplicationUserDto user);
    Task HandleWebhookAsync(string reference, decimal amount);
}
}