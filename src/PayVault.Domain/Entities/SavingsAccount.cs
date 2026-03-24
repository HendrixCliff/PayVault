namespace PayVault.Domain.Entities
{
    public class SavingsAccount
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Balance { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public decimal InterestRate { get; private set; } = 0.05m; 

        public SavingsAccount() { }

        public SavingsAccount(string userId, decimal initialBalance, decimal interestRate)
        {
            UserId = userId;
            Balance = initialBalance;
            InterestRate = interestRate;
        }

       
        public decimal CalculateInterest()
        {
            return Balance * InterestRate;
        }

      
        public void AddInterest(decimal interest)
        {
            if (interest < 0)
                throw new InvalidOperationException("Interest cannot be negative");

            Balance += interest;
        }
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Deposit amount must be greater than zero");

            Balance += amount;
        }
    }
}