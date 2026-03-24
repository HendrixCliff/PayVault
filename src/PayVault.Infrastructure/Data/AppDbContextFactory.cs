using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace PayVault.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "PayVaultDb";
            var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
            var sslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE") ?? "Require";
            var channelBinding = Environment.GetEnvironmentVariable("DB_CHANNEL_BINDING") ?? "Disable";

            var connectionString = $"Host={host};Database={database};Username={username};Password={password};SslMode={sslMode};Trust Server Certificate=false;ChannelBinding={channelBinding}";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}