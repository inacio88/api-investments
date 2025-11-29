using core.Entities;

namespace application.Services
{
    public interface IInvestmentService
    {
        Task<Investment> CreateInvestmentAsync(string ownerId, decimal amount, DateTime creationDate);
        Task WithdrawInvestmentAsync(Guid investmentId, DateTime withdrawalDate);
        Task<Investment?> GetInvestmentByIdAsync(Guid id);
        Task<List<Investment>> GetInvestmentsByOwnerAsync(string ownerId, int page, int pageSize);
    }
}