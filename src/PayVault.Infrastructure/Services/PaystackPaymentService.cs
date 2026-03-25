using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Entities;
using PayVault.Infrastructure.Identity;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using PayVault.Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using PayVault.Application.DTOs.Auth;
using System.Text;

namespace PayVault.Infrastructure.Services
{


public class PaystackPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IRepository<Loan> _loanRepository;
    private readonly IConfiguration _config;

   public PaystackPaymentService(HttpClient httpClient, IRepository<Loan> loanRepository, IConfiguration config)
    {
        _httpClient = httpClient;
        _loanRepository = loanRepository;
        _config = config;
    }

   public async Task<string> DisburseLoanAsync(Loan loan, ApplicationUserDto user) 
    {
       
        string recipientCode = await CreateTransferRecipientAsync(user.BankCode, user.AccountNumber, user.FullName);

       
        var payload = new
        {
            source = "balance",
            reason = $"Loan disbursement for {user.FullName}",
            amount = (int)(loan.Amount * 100),
            recipient = recipientCode,
            reference = Guid.NewGuid().ToString("N")
        };

        var response = await _httpClient.PostAsync(
            "transfer",
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();

        dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

        return result?.data?.reference;
    }

    private async Task<string> CreateTransferRecipientAsync(string bankCode, string accountNumber, string accountName)
    {
        var payload = new
        {
            type = "nuban",
            name = accountName,
            account_number = accountNumber,
            bank_code = bankCode,
            currency = "NGN"
        };

        var response = await _httpClient.PostAsync(
            "transferrecipient",
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
        dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        return result?.data?.recipient_code;
    }

 
    public async Task HandleWebhookAsync(string reference, decimal amount)
    {
        var loan = (await _loanRepository.GetAllAsync())
            .FirstOrDefault(l => l.TransactionReference == reference);

        if (loan == null) throw new InvalidOperationException("Loan not found");

        loan.ApplyRepayment(amount);
        await _loanRepository.UpdateAsync(loan);
    }
}
}