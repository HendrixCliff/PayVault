using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PayVault.Domain.Entities;
using PayVault.Infrastructure.Data;
using Microsoft.AspNetCore.Identity; 
  using PayVault.Infrastructure.Identity;

namespace PayVault.Infrastructure.Data
{
public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.EnableSensitiveDataLogging()
                  .EnableDetailedErrors();
}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Loan>().ToTable("Loans");
            modelBuilder.Entity<Loan>().Property(l => l.Id).HasColumnName("Id");
            modelBuilder.Entity<Loan>().Property(l => l.UserId).HasColumnName("UserId");
            modelBuilder.Entity<Loan>().Property(l => l.Amount).HasColumnName("Amount");
            modelBuilder.Entity<Loan>().Property(l => l.InterestRate).HasColumnName("InterestRate");
            modelBuilder.Entity<Loan>().Property(l => l.Installments).HasColumnName("Installments");
            modelBuilder.Entity<Loan>().Property(l => l.CreatedAt).HasColumnName("CreatedAt");
            modelBuilder.Entity<Loan>().Property(l => l.Status).HasColumnName("Status");
        
         modelBuilder.Entity<IdentityRole>().HasData(
        new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
        new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
    );
     modelBuilder.Entity<ApplicationUser>(entity =>
    {
        entity.Property(e => e.DateOfBirth)
              .HasColumnType("date"); 
        
        entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp with time zone")
              .HasDefaultValueSql("NOW()");
    });
            // Configure LoanPayments Table
            modelBuilder.Entity<LoanPayment>().ToTable("LoanPayments");
            modelBuilder.Entity<LoanPayment>().Property(lp => lp.Id).HasColumnName("Id");
            modelBuilder.Entity<LoanPayment>().Property(lp => lp.LoanId).HasColumnName("LoanId");
            modelBuilder.Entity<LoanPayment>().Property(lp => lp.Amount).HasColumnName("Amount");
            modelBuilder.Entity<LoanPayment>().Property(lp => lp.PaidAt).HasColumnName("PaidAt");
            modelBuilder.Entity<LoanPayment>().Property(lp => lp.Status).HasColumnName("Status");

            // Configure SavingsAccounts Table
            modelBuilder.Entity<SavingsAccount>().ToTable("SavingsAccounts");
            modelBuilder.Entity<SavingsAccount>().Property(s => s.Id).HasColumnName("Id");
            modelBuilder.Entity<SavingsAccount>().Property(s => s.UserId).HasColumnName("UserId");
            modelBuilder.Entity<SavingsAccount>().Property(s => s.Balance).HasColumnName("Balance");
            modelBuilder.Entity<SavingsAccount>().Property(s => s.InterestRate).HasColumnName("InterestRate");
            modelBuilder.Entity<SavingsAccount>().Property(s => s.CreatedAt).HasColumnName("CreatedAt");

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        }
    }
}