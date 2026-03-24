using PayVault.Domain.Entities;

namespace PayVault.Application.Interfaces.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdWithPaymentsAsync(Guid id);
        Task<Loan?> GetByIdAsync(Guid id, bool includeRelated = false);
        Task<List<Loan>> GetPendingLoansAsync();
    }
}