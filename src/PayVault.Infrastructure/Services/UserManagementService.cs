using Microsoft.AspNetCore.Identity;
using PayVault.Application.DTOs.Auth;
using PayVault.Infrastructure.Identity;
using PayVault.Application.Interfaces.Services;

namespace PayVault.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Guid> RegisterAsync(RegisterDto request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                IsAccountActive = true 
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            return Guid.Parse(user.Id);
        }

        public async Task<ApplicationUserDto?> GetByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null ? await ToDtoAsync(user) : null;
        }

        public async Task<ApplicationUserDto?> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null ? await ToDtoAsync(user) : null;
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return string.Empty;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "User";
        }

        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.IsAccountActive ?? false;
        }

        
        private async Task<ApplicationUserDto> ToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new ApplicationUserDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                IsAccountActive = user.IsAccountActive,
                Role = roles.FirstOrDefault() ?? "User"
            };
        }
    }
}