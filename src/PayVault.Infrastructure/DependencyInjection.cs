using Microsoft.Extensions.DependencyInjection;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;
using PayVault.Infrastructure.Repositories;
using PayVault.Infrastructure.Services;

namespace PayVault.Infrastructure
{
    public static class DependencyInjection
    {
       public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    services.AddScoped<LoanRepository>();

    services.AddScoped<IUserManagementService, UserManagementService>();

    return services;
}
    }
}