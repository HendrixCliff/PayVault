using Microsoft.AspNetCore.Mvc;
using PayVault.Domain.Entities;
using PayVault.Infrastructure.Identity;
using PayVault.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace PayVault.API.Controllers
{
    [ApiController]
    [Route("api/paystack/webhook")]
    public class PaystackWebhookController : ControllerBase
    {
        private readonly IRepository<Loan> _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaystackWebhookController(
            IRepository<Loan> loanRepository,
            UserManager<ApplicationUser> userManager)
        {
            _loanRepository = loanRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] dynamic payload)
        {
            try
            {
                string eventType = payload.@event;

                // Only process repayments (charge.success) or disbursement confirmations (transfer.success)
                if (eventType != "charge.success" && eventType != "transfer.success")
                    return Ok();

                string reference = payload.data.reference;
                decimal amount = payload.data.amount / 100m; // Paystack sends kobo
                string customerEmail = payload.data.customer.email;

                // Lookup user
                var user = await _userManager.FindByEmailAsync(customerEmail);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Lookup loan by transaction reference
                var loans = await _loanRepository.GetAllAsync();
                var loan = loans.FirstOrDefault(l => l.TransactionReference == reference);
                if (loan == null)
                    return NotFound(new { message = "Loan not found for this transaction" });

                // Apply repayment to loan
                loan.ApplyRepayment(amount);

                await _loanRepository.UpdateAsync(loan);

                return Ok(new
                {
                    message = "Repayment applied successfully",
                    loanId = loan.Id,
                    remainingBalance = loan.RemainingBalance,
                    status = loan.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                // Optional: log exception to your logging system
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}