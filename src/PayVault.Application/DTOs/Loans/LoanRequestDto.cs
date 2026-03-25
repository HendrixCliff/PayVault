namespace PayVault.Application.DTOs.Loans {
    public class LoanRequestDto
{
    public decimal Amount { get; set; }
    public string BankCode { get; set; } = string.Empty;      
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;   
}
}