namespace PayVault.Application.Common.Interfaces
{
    public interface IApplicationUser
    {
        string Id { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
        bool IsEmailVerified { get; }
        bool IsAccountActive { get; }
    }
}