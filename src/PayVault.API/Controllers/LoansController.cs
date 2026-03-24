using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayVault.Application.DTOs.Loans;
using PayVault.Application.UseCases.Loans;
using PayVault.Application.UseCases.Loans.Queries;
using PayVault.Application.UseCases.Loans.Queries.GetLoan;
using PayVault.Domain.Enums;
using System.Security.Claims;

namespace PayVault.API.Controllers
{
    [ApiController]
    [Route("api/loans")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LoanController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
        }

       

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoan(Guid loanId)
        {
            var query = new GetLoanQuery { LoanId = loanId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        
        [HttpGet("pending/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LoanDto>>> GetPendingLoansAdmin()
        {
            var query = new GetLoansByStatusQuery { Status = LoanStatus.Pending };  // ✅ No UserId
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("approved/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LoanDto>>> GetApprovedLoansAdmin()
        {
            var query = new GetLoansByStatusQuery { Status = LoanStatus.Approved };  // ✅ No UserId
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("rejected/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LoanDto>>> GetRejectedLoansAdmin()
        {
            var query = new GetLoansByStatusQuery { Status = LoanStatus.Rejected };  // ✅ No UserId
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("all/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LoanDto>>> GetAllLoansAdmin()
        {
            var query = new GetUserLoansQuery();  // ✅ No UserId = ALL loans
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}