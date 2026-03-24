using AutoMapper;
using MediatR;
using PayVault.Application.DTOs.Loans;
using PayVault.Application.Interfaces.Services;
using PayVault.Domain.Exceptions;
using PayVault.Application.Interfaces.Repositories;


namespace PayVault.Application.UseCases.Loans.Queries.GetLoan
{
    public class GetLoanQueryHandler : IRequestHandler<GetLoanQuery, GetLoanResult>
    {
       private readonly ILoanRepository _loanRepository;
        private readonly IUserManagementService _userService;
        private readonly IMapper _mapper;

        public GetLoanQueryHandler(
              ILoanRepository loanRepository,
            IUserManagementService userService,
            IMapper mapper)
        {
           _loanRepository = loanRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<GetLoanResult> Handle(GetLoanQuery request, CancellationToken cancellationToken)
{
   
    var loan = await _loanRepository.GetByIdWithPaymentsAsync(request.LoanId);
    
    if (loan == null)
        throw new NotFoundException($"Loan {request.LoanId} not found");

    var user = await _userService.GetByIdAsync(loan.UserId);
    
    var result = _mapper.Map<GetLoanResult>(loan);
    result.UserEmail = user?.Email ?? "Unknown";
    
    result.Payments = loan.Payments != null 
        ? _mapper.Map<List<LoanPaymentDto>>(loan.Payments)
        : new List<LoanPaymentDto>();

    return result;
}
    }
}