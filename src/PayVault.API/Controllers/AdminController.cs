using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PayVault.Application.UseCases.Loans.Commands.ApproveLoan;
using PayVault.Application.UseCases.Loans.Commands.RejectLoan;
using PayVault.Application.UseCases.Loans.Queries.GetLoan; 
using PayVault.Application.UseCases.Loans.Queries.GetLoanPayments;
using PayVault.Application.DTOs.Loans;  
using PayVault.API.Extensions;


namespace PayVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

 

    
        [HttpGet("loans/{loanId}")]
        public async Task<ActionResult<GetLoanResult>> GetLoan(Guid loanId)
        {
            var query = new GetLoanQuery { LoanId = loanId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("loans/{loanId}/payments")]
        public async Task<ActionResult<GetLoanPaymentsResult>> GetLoanPayments(Guid loanId)
        {
            var query = new GetLoanPaymentsQuery { LoanId = loanId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}