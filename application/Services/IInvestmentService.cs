using application.DTOs;
using core.Entities;

namespace application.Services
{
    public interface IInvestmentService
    {
        Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentInput input);
        Task WithdrawInvestmentAsync(Guid investmentId, DateTime withdrawalDate);
        Task<InvestmentDto?> GetInvestmentByIdAsync(Guid id);
        Task<PaginatedResult<InvestmentDto>> GetInvestmentsByOwnerAsync(string ownerId, int page, int pageSize);
    }
}