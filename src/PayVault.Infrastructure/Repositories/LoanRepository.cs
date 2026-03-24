using Microsoft.EntityFrameworkCore;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;
using PayVault.Infrastructure.Data;

namespace PayVault.Infrastructure.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Loan?> GetByIdWithPaymentsAsync(Guid id)
        {
            return await _context.Loans
                .Include(l => l.Payments)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public override async Task<Loan?> GetByIdAsync(Guid id, bool includeRelated = false)
        {
            var query = _context.Loans.AsQueryable();
            if (includeRelated)
                query = query.Include(l => l.Payments);
            return await query.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Loan>> GetPendingLoansAsync()
        {
            return await _context.Loans
                .Where(l => l.Status == LoanStatus.Pending)
                .ToListAsync();
        }
    }
}