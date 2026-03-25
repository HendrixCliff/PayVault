using PayVault.Application.Common.Interfaces;
using PayVault.Application.DTOs.Auth;

namespace PayVault.Application.Common.Adapters
{
    public class ApplicationUserWrapper : IApplicationUser
    {
        private readonly ApplicationUserDto _dto;

        public ApplicationUserWrapper(ApplicationUserDto dto)
        {
            _dto = dto;
        }

        public string Id => _dto.Id.ToString();
        public string Email => _dto.Email;
        public string FirstName => _dto.FirstName;
        public string LastName => _dto.LastName;
        public DateTime DateOfBirth => DateTime.MinValue; // DTO does not have DOB
        public bool IsEmailVerified => true;              // default
        public bool IsAccountActive => _dto.IsAccountActive;
    }
}