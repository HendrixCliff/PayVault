using MediatR;
using PayVault.Application.DTOs.Loans;

namespace PayVault.Application.UseCases.Loans.Queries
{
    public class GetUserLoansQuery : IRequest<List<LoanDto>>
    {
       public Guid? UserId { get; set; } 
    }
}