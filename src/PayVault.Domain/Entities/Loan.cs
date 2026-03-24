using PayVault.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace PayVault.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal InterestRate { get; private set; }
        public int Installments { get; private set; }

         [Column(TypeName = "decimal(18,2)")]
        public decimal? RemainingBalance { get; private set; }
        public decimal MonthlyPayment { get; private set; }
        public DateTime CreatedAt { get; private set; }
         public DateTime UpdatedAt { get; private set; }
        public LoanStatus Status { get; private set; }
         public DateTime? ApprovedAt { get; private set; }
           public string? Reason { get; private set; }
    public DateTime? RejectedAt { get; private set; }
        public List<LoanPayment> Payments { get; private set; } = new();
            public string? TransactionReference { get; private set; }
          public string BankCode { get; private set; }
        public string AccountNumber { get; private set; }
        public string AccountName { get; private set; }
                private Loan() { }

       public void SetBankDetails(string bankCode, string accountNumber, string accountName)
        {
            BankCode = bankCode;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }
        public static Loan Create(Guid userId, decimal amount, decimal interestRate, int installments)
        {
            var totalInterest = amount * interestRate / 100;
            var totalAmount = amount + totalInterest;
            var monthlyPayment = totalAmount / installments;

            return new Loan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                InterestRate = interestRate,
                Installments = installments,
                MonthlyPayment = monthlyPayment,
                CreatedAt = DateTime.UtcNow,
                Status = LoanStatus.Pending
            };
        }

        
        public decimal CalculateRemainingBalance()
        {
            if (Status != LoanStatus.Approved)
                return Amount;

            var totalPaid = Payments?.Where(p => p.Status == PaymentStatus.Paid)
                                   .Sum(p => p.Amount) ?? 0m;
            
            return RemainingBalance ?? Amount - totalPaid;
        }
        public void Activate()
        {
            if (Status != LoanStatus.Approved)
                throw new InvalidOperationException("Loan must be approved before activation");

            Status = LoanStatus.Active;
        }

        public void AddPayment(decimal amount)
        {
            if (Status != LoanStatus.Active)
                throw new InvalidOperationException("Loan is not active");

           
            Payments.Add(LoanPayment.Create(Id, amount));
            UpdatedAt = DateTime.UtcNow;
        }
    public void ApplyRepayment(decimal amount)
    {
        if (Status != LoanStatus.Active && Status != LoanStatus.Approved)
            throw new InvalidOperationException("Cannot apply repayment to inactive loan");

        RemainingBalance = (RemainingBalance ?? Amount) - amount;

        if (RemainingBalance <= 0)
        {
            RemainingBalance = 0;
            Status = LoanStatus.Completed;
        }

        UpdatedAt = DateTime.UtcNow;
    }


    public void SetTransactionReference(string reference)
    {
        TransactionReference = reference;
    }
                public void Approve()
        {
            if (Status != LoanStatus.Pending)
                throw new InvalidOperationException("Only pending loans can be approved");
                
            Status = LoanStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
            RemainingBalance = Amount * (1 + InterestRate / 100);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(string reason)
        {
            Status = LoanStatus.Rejected;
            Reason = reason;
            RejectedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}