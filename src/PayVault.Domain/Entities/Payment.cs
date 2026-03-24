using System.ComponentModel.DataAnnotations;
using PayVault.Domain.Enums;
using PayVault.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema; 


namespace PayVault.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid LoanId { get; private set; }
        public Guid UserId { get; private set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }
        
        public DateTime PaymentDate { get; private set; }
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public string? TransactionId { get; private set; }
        public string? PaymentMethod { get; private set; } // Card, Bank, Wallet

        // Navigation
        public virtual Loan Loan { get; private set; } = null!;

        private Payment() { } // EF Core

        public static Payment Create(Guid loanId, Guid userId, decimal amount, string paymentMethod)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0");

            return new Payment
            {
                LoanId = loanId,
                UserId = userId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = paymentMethod
            };
        }

        public void MarkAsPaid(string transactionId)
        {
            Status = PaymentStatus.Paid;
            TransactionId = transactionId;
        }

        public void MarkAsFailed(string reason)
        {
            Status = PaymentStatus.Failed;
        }
    }

   
}