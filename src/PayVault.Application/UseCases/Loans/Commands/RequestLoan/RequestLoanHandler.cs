using MediatR;
using PayVault.Application.Common.Interfaces;
using PayVault.Application.DTOs.Auth;     
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Entities;
using PayVault.Application.Common.Adapters;

namespace PayVault.Application.UseCases.Loans.Commands.RequestLoan
{
    public class RequestLoanHandler : IRequestHandler<RequestLoanCommand, LoanResult>
    {
        private readonly IRepository<Loan> _loanRepository;
        private readonly ILoanEligibilityService _eligibilityService;
        private readonly IPaymentService _paymentService;
        private readonly IUserManagementService _userService;

        public RequestLoanHandler(
            IRepository<Loan> loanRepository,
            ILoanEligibilityService eligibilityService,
            IPaymentService paymentService,
            IUserManagementService userService)
        {
            _loanRepository = loanRepository;
            _eligibilityService = eligibilityService;
            _paymentService = paymentService;
            _userService = userService;
        }

       public async Task<LoanResult> Handle(RequestLoanCommand request, CancellationToken ct)
{
  
    Guid userGuid = Guid.Parse(request.UserId);

  
    ApplicationUserDto? userDto = await _userService.GetByIdAsync(userGuid);
    if (userDto == null)
        throw new InvalidOperationException("User not found");

    if (!userDto.IsAccountActive)
        throw new InvalidOperationException("User account is inactive");

   
    (bool isEligible, string reason) = await _eligibilityService.IsEligibleAsync(
    new ApplicationUserWrapper(userDto), request.Amount
);

    if (!isEligible)
    {
        return new LoanResult
        {
            Message = $"Loan request denied: {reason}"
        };
    }

 
    var loan = Loan.Create(userGuid, request.Amount, request.InterestRate, request.Installments);
    loan.SetBankDetails(request.BankCode, request.AccountNumber, request.AccountName);

    await _loanRepository.AddAsync(loan);

    loan.Approve();
    await _loanRepository.UpdateAsync(loan);

    
    var reference = await _paymentService.DisburseLoanAsync(loan, userDto);

    loan.SetTransactionReference(reference);
    await _loanRepository.UpdateAsync(loan);

    return new LoanResult
    {
        LoanId = loan.Id,
        Message = "Loan approved and disbursed",
        PaymentReference = reference
    };
}
    }
}