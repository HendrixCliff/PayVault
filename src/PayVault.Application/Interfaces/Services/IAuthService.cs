namespace PayVault.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterUser(string email, string password, string firstName, string lastName);
        Task<string> Login(string email, string password);
        Task<string> GenerateToken(string userId);
        Task<string> FindUserByEmail(string email);
        Task<bool> ValidatePassword(string email, string password);
        string GenerateRefreshToken();
    }
}