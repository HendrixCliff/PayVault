using MediatR;

namespace PayVault.Application.UseCases.Savings.Commands.CalculateInterest
{
    public class CalculateInterestCommand : IRequest
{
    public Guid AccountId { get; set; }
}
}