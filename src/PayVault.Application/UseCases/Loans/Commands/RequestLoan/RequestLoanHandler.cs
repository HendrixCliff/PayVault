using MediatR;
using PayVault.Application.Interfaces.Repositories;
using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Entities;
using PayVault.Infrastructure.Identity;

namespace PayVault.Application.UseCases.Loans.Commands.RequestLoan
{
    public class RequestLoanHandler : IRequestHandler<RequestLoanCommand, LoanResult>
    {
        private readonly IRepository<Loan> _loanRepository;
        private readonly ILoanEligibilityService _eligibilityService;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestLoanHandler(
            IRepository<Loan> loanRepository,
            ILoanEligibilityService eligibilityService,
            IPaymentService paymentService,
            UserManager<ApplicationUser> userManager)
        {
            _loanRepository = loanRepository;
            _eligibilityService = eligibilityService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        public async Task<LoanResult> Handle(RequestLoanCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) throw new InvalidOperationException("User not found");

            var eligibility = await _eligibilityService.IsEligibleAsync(user, request.Amount);
            if (!eligibility.IsEligible)
                return new LoanResult { Message = $"Loan request denied: {eligibility.Reason}" };

            // Create loan
            var loan = Loan.Create(user.Id, request.Amount, request.InterestRate, request.Installments);
            loan.SetBankDetails(request.BankCode, request.AccountNumber, request.AccountName);
            await _loanRepository.AddAsync(loan);

            // Automatic approval
            loan.Approve();
            await _loanRepository.UpdateAsync(loan);

            // Disburse via Paystack
            string reference = await _paymentService.DisburseLoanAsync(loan, user);
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