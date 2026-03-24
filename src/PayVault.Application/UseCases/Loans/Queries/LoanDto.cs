using PayVault.Domain.Enums;

namespace PayVault.Application.DTOs.Loans
{
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Installments { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}