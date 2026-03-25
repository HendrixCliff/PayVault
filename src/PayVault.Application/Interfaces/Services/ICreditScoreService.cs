namespace PayVault.Application.Interfaces.Services
{
    public interface ICreditScoreService
    {
        Task<int> GetScoreAsync(string userId);
    }
}