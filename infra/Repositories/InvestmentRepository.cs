using core.Entities;
using core.Filters;
using core.Repositories;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly ApplicationDbContext _context;

        public InvestmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Investment investment)
        {
            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(InvestmentFilter filter)
        {
            return await _context.Investments.AnyAsync(e => e.Id == filter.Id && e.OwnerId == filter.OwnerId);
        }

        public async Task<Investment?> GetByIdAsync(InvestmentFilter filter)
        {
            return await _context.Investments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == filter.Id && e.OwnerId == filter.OwnerId);
        }

        public async Task<IEnumerable<Investment>> GetByOwnerIdAsync(InvestmentFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.OwnerId))
                throw new ArgumentException("OwnerId não pode ser nulo ou vazio.");

            return await _context.Investments
                .AsNoTracking()
                .Where(e => e.OwnerId == filter.OwnerId)
                .OrderBy(e => e.CreationDate)
                .Skip(filter.Skip)
                .Take(filter.PageSizeClamped)
                .ToListAsync();
        }

        public async Task<long> CountByOwnerIdAsync(InvestmentFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.OwnerId))
                throw new ArgumentException("OwnerId não pode ser nulo ou vazio.");

            return await _context.Investments
                .AsNoTracking()
                .LongCountAsync(e => e.OwnerId == filter.OwnerId);
        }

        public async Task UpdateAsync(Investment investment)
        {
            _context.Investments.Update(investment);
            await _context.SaveChangesAsync();
        }
    }
}