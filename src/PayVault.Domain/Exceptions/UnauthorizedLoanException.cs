namespace PayVault.Domain.Exceptions
{
    public class UnauthorizedLoanException : Exception
    {
        public UnauthorizedLoanException(string message) : base(message)
        {
        }
    }
}