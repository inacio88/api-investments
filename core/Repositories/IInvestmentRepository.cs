using core.Entities;

namespace core.Repositories
{
    public interface IInvestmentRepository
    {
        Task CreateAsync(Investment investment);
        Task UpdateAsync(Investment investment);
        Task DeleteAsync(Guid id);
        Task<Investment> GetByIdAsync(Guid id);
        Task<List<Investment>> GetByFilterAsync(Guid id);

    }
}