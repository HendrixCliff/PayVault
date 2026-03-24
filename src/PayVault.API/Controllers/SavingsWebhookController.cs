using Microsoft.AspNetCore.Mvc;
using PayVault.Application.Interfaces.Services;
using Newtonsoft.Json;
using System.Text;

namespace PayVault.API.Controllers
{
    [ApiController]
    [Route("api/webhook/savings")]
    public class SavingsWebhookController : ControllerBase
    {
        private readonly ISavingsPaymentService _savingsPaymentService;
        private readonly IConfiguration _configuration;

        public SavingsWebhookController(ISavingsPaymentService savingsPaymentService, IConfiguration configuration)
        {
            _savingsPaymentService = savingsPaymentService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook()
        {
            
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

           
            if (!VerifySignature(Request.Headers["x-paystack-signature"], json))
            {
                return Unauthorized("Invalid signature");
            }

    
            dynamic data = JsonConvert.DeserializeObject(json);
            string eventType = data.@event;
            dynamic eventData = data.data;

           
            if (eventType == "charge.success")
            {
                string reference = eventData.reference;
                decimal amount = eventData.amount / 100m; // Paystack sends amount in kobo

                try
                {
                    await _savingsPaymentService.HandleWebhookAsync(reference, amount);
                    return Ok();
                }
                catch (Exception ex)
                {
                  
                    return BadRequest(ex.Message);
                }
            }

            
            return Ok();
        }

        private bool VerifySignature(string signatureHeader, string payload)
        {
            var secretKey = _configuration["Paystack:SecretKey"];
            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return hashString == signatureHeader;
        }
    }
}