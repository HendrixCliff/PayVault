namespace PayVault.Application.DTOs.Loans
{
    public class LoanPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsPaid { get; set; }
        public decimal RemainingBalance { get; set; } 
    }
}