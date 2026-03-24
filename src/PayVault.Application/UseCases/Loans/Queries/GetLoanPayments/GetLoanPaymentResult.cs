using PayVault.Domain.Entities;
using PayVault.Domain.Enums;
using PayVault.Application.DTOs.Loans;

namespace PayVault.Application.UseCases.Loans.Queries.GetLoanPayments
{
    public class GetLoanPaymentsResult
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Installments { get; set; }
        public LoanStatus Status { get; set; }
        public decimal RemainingBalance { get; set; }
        public List<LoanPaymentDto> Payments { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

   
}