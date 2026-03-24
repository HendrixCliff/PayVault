using PayVault.Application.DTOs.Auth;

namespace PayVault.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<Guid> RegisterAsync(RegisterDto request);

        Task<ApplicationUserDto?> GetByIdAsync(Guid userId);

        Task<ApplicationUserDto?> GetByEmailAsync(string email);

        Task<string> GetUserRoleAsync(Guid userId);

        Task<bool> IsUserActiveAsync(Guid userId);
    }
}