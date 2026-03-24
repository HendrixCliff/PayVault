namespace PayVault.Application.DTOs.Loans {
    public class LoanRequestDto
{
    public decimal Amount { get; set; }
    public string BankCode { get; set; }      
    public string AccountNumber { get; set; } 
    public string AccountName { get; set; }   
}
}