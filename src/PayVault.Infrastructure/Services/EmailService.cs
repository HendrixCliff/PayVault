using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using PayVault.Application.Interfaces.Services;

namespace PayVault.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var smtp = new SmtpClient
                {
                    Host = _settings.Host,
                    Port = _settings.Port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password)
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.From, "PayVault"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml  
                };

                mail.To.Add(to);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed to {to}: {ex.Message}");
                throw;
            }
        }
    }
}