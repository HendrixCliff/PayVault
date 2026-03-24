using PayVault.Domain.Enums;

namespace PayVault.Domain.Entities
{
    public class LoanPayment
    {
        public Guid Id { get; private set; }
        public Guid LoanId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; set; }
        
        private LoanPayment() { }

        public static LoanPayment Create(Guid loanId, decimal amount)
        {
            return new LoanPayment
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                Amount = amount,
                PaidAt = DateTime.UtcNow,
                Status = PaymentStatus.Pending
            };
        }

        public void MarkAsPaid() => Status = PaymentStatus.Paid;
        public void MarkAsFailed() => Status = PaymentStatus.Failed;
    }


}