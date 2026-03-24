using MediatR;
using PayVault.Domain.Enums;
using PayVault.Application.DTOs.Loans;

namespace PayVault.Application.UseCases.Loans.Queries
{
    public class GetLoansByStatusQuery : IRequest<List<LoanDto>>
    {
       public Guid? UserId { get; set; } 
        public LoanStatus Status { get; set; }
    }
}