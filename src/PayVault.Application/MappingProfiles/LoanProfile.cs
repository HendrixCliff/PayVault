using PayVault.Application.DTOs.Loans;
using AutoMapper;
using PayVault.Application.UseCases.Loans.Queries.GetLoan;
using PayVault.Domain.Entities;
using PayVault.Domain.Enums;

namespace PayVault.Application.MappingProfiles
{
    public class LoanProfile : Profile
    {
     public LoanProfile()
        {
            CreateMap<Loan, GetLoanResult>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.RemainingBalance, opt => opt.MapFrom(src => src.CalculateRemainingBalance()))  // ✅ Now compiles!
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore());

            CreateMap<Payment, LoanPaymentDto>()
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.Status == PaymentStatus.Paid));
        }
    }
}