using System.Text.Json.Serialization;
using PayVault.Application.DTOs.Loans;

namespace PayVault.Application.UseCases.Loans.Queries.GetLoan
{
    public class GetLoanResult
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Installments { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal RemainingBalance { get; set; }
        public List<LoanPaymentDto> Payments { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
         public DateTime? RejectedAt { get; set; }    
    public string? RejectionReason { get; set; }
    }

}