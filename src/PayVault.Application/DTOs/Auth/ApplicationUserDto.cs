

namespace PayVault.Application.DTOs.Auth
{
        public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsAccountActive { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}