using core.Entities;
using core.Filters;

namespace core.Repositories
{
    public interface IInvestmentRepository
    {
        Task<Investment?> GetByIdAsync(InvestmentFilter filter);
        Task<IEnumerable<Investment>> GetByOwnerIdAsync(InvestmentFilter filter);
        Task<long> CountByOwnerIdAsync(InvestmentFilter filter);
        Task AddAsync(Investment investment);
        Task UpdateAsync(Investment investment);
        Task<bool> ExistsAsync(InvestmentFilter filter);

    }
}