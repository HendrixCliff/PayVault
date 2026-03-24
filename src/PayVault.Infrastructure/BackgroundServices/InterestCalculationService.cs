using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayVault.Application.UseCases.Savings.Commands.CalculateInterest;
using MediatR;

namespace PayVault.Infrastructure.BackgroundServices
{
    public class InterestCalculationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InterestCalculationService> _logger;

        public InterestCalculationService(
            IServiceProvider serviceProvider, 
            ILogger<InterestCalculationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("InterestCalculationService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CalculateDailyInterest(stoppingToken);
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating daily interest");
                }
            }
        }

        private async Task CalculateDailyInterest(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            _logger.LogInformation("Starting daily interest calculation");
            
            // TODO: Implement GetActiveSavingsAccountsQuery
            var activeAccountIds = await GetActiveSavingsAccountIds(scope);
            
            foreach (var accountId in activeAccountIds)
            {
                try
                {
                    await mediator.Send(
                        new CalculateInterestCommand { AccountId = accountId }, 
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to calculate interest for account {AccountId}", accountId);
                }
            }
            
            _logger.LogInformation("Daily interest calculation completed");
        }

        private Task<List<Guid>> GetActiveSavingsAccountIds(IServiceScope scope)
        {
            return Task.FromResult(new List<Guid>());
        }
    }
}