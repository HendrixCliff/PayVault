using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Entities;
using Newtonsoft.Json;
using System.Text;

namespace PayVault.Infrastructure.Services
{
    public class SavingsPaymentService : ISavingsPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<SavingsAccount> _repository;

        public SavingsPaymentService(HttpClient httpClient, IRepository<SavingsAccount> repository)
        {
            _httpClient = httpClient;
            _repository = repository;
        }

       
     public async Task<(string PaymentReference, string PaymentUrl)> InitializeDepositAsync(SavingsAccount account, decimal amount)
{
    var payload = new
    {
        email = account.OwnerEmail,       // Savings account owner email
        amount = (int)(amount * 100),    // Paystack expects amount in kobo
        currency = "NGN",
        reference = Guid.NewGuid().ToString("N")
    };

    var response = await _httpClient.PostAsync(
        "transaction/initialize",
        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
    );

    response.EnsureSuccessStatusCode();

    dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

    string reference = result?.data?.reference;
    string authorizationUrl = result?.data?.authorization_url;

    
    account.PendingTransactionReference = reference;
    await _repository.UpdateAsync(account);

    return (reference, authorizationUrl);
}

      public async Task<SavingsAccount> GetAccountByIdAsync(Guid accountId)
{
    var account = (await _repository.GetAllAsync())
                  .FirstOrDefault(a => a.Id == accountId);

    if (account == null)
        throw new InvalidOperationException("Savings account not found");

    return account;
}
        public async Task HandleWebhookAsync(string reference, decimal amount)
        {
            var account = (await _repository.GetAllAsync())
                .FirstOrDefault(a => a.PendingTransactionReference == reference);

            if (account == null) throw new InvalidOperationException("Savings account transaction not found");

            account.Deposit(amount);
            account.PendingTransactionReference = null;

            await _repository.UpdateAsync(account);
        }
    }
}