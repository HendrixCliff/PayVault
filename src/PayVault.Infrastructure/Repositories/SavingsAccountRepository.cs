using PayVault.Application.Interfaces.Repositories;
using PayVault.Domain.Entities;
using PayVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace PayVault.Infrastructure.Repositories
{
    public class SavingsAccountRepository : Repository<SavingsAccount>, ISavingsAccountRepository
    {
        public SavingsAccountRepository(AppDbContext context) : base(context)
        {
        }

       
        public async Task<SavingsAccount?> GetByUserIdAsync(Guid userId)
        {
            return await _context.SavingsAccounts
                .FirstOrDefaultAsync(s => s.UserId == userId.ToString());
        }

        public async Task<List<SavingsAccount>> GetActiveAccountsAsync()
        {
            return await _context.SavingsAccounts
                .Where(s => s.Balance > 0)
                .ToListAsync();
        }
    }

    public interface ISavingsAccountRepository : IRepository<SavingsAccount>
    {
        Task<SavingsAccount?> GetByUserIdAsync(Guid userId);
        Task<List<SavingsAccount>> GetActiveAccountsAsync();
    }
}