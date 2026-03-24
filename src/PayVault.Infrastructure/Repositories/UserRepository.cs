using Microsoft.AspNetCore.Identity;
using PayVault.Infrastructure.Interfaces;
using PayVault.Infrastructure.Identity;
using PayVault.Infrastructure.Data;

namespace PayVault.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> CreateUserAsync(
            string email,
            string firstName,
            string lastName,
            string fullName,
            string password,
            string role,
            DateTime dateOfBirth)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                FullName = fullName,
              DateOfBirth = dateOfBirth.Date, 
        CreatedAt = DateTime.UtcNow, 
                IsEmailVerified = false,
                IsAccountActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return user;
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}