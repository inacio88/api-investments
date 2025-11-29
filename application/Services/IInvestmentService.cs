using application.DTOs;
using core.Entities;
using core.Filters;

namespace application.Services
{
    public interface IInvestmentService
    {
        Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentInput input);
        Task WithdrawInvestmentAsync(Guid investmentId, string ownerId, DateTime withdrawalDate);
        Task<InvestmentDto?> GetInvestmentByIdAsync(InvestmentFilter filter);
        Task<PaginatedResult<InvestmentDto>> GetInvestmentsByOwnerAsync(InvestmentFilter filter);
    }
}