using core.Entities;

namespace core.Repositories
{
    public interface IInvestmentRepository
    {
        Task<Investment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Investment>> GetByOwnerIdAsync(string ownerId, int page, int pageSize);
        Task<long> CountByOwnerIdAsync(string ownerId);
        Task AddAsync(Investment investment);
        Task UpdateAsync(Investment investment);
        Task<bool> ExistsAsync(Guid id);

    }
}