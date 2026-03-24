using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using PayVault.Domain.Entities;

namespace  PayVault.Infrastructure.Identity 
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsAccountActive { get; set; } = true;
         public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

      public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
      public virtual ICollection<SavingsAccount> SavingsAccounts { get; set; } = new List<SavingsAccount>();
    }
}