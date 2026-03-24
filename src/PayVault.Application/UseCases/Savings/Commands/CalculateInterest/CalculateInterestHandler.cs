using PayVault.Application.Interfaces.Repositories;
using MediatR;
using PayVault.Domain.Entities;

namespace PayVault.Application.UseCases.Savings.Commands.CalculateInterest
{
    public class CalculateInterestHandler : IRequestHandler<CalculateInterestCommand>
    {
        private readonly IRepository<SavingsAccount> _repository;

      
        public CalculateInterestHandler(IRepository<SavingsAccount> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CalculateInterestCommand request, CancellationToken ct)
{
    var account = await _repository.GetByIdAsync(request.AccountId);

    if (account is null)
        throw new InvalidOperationException("Savings account not found");

    var interest = account.CalculateInterest(); 
    account.AddInterest(interest);

    await _repository.UpdateAsync(account);
}
    }
}