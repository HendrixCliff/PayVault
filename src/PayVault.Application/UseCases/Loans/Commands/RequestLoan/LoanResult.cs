namespace PayVault.Application.UseCases.Loans.Commands.RequestLoan {
    public class LoanResult
{
    public Guid LoanId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
}
}