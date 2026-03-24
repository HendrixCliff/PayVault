using PayVault.Infrastructure.Identity;

namespace PayVault.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> CreateUserAsync(
            string email,
            string firstName,
            string lastName,
            string fullName,
            string password,
            string role,
            DateTime dateOfBirth
        );

        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByIdAsync(string id);
    }
}