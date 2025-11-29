using core.Entities;
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

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Investments.AnyAsync(e => e.Id == id);
        }

        public async Task<Investment?> GetByIdAsync(Guid id)
        {
            return await _context.Investments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Investment>> GetByOwnerIdAsync(string ownerId, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("OwnerId não pode ser nulo ou vazio.", nameof(ownerId));

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return await _context.Investments
                .AsNoTracking()
                .Where(e => e.OwnerId == ownerId)
                .OrderBy(e => e.CreationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> CountByOwnerIdAsync(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("OwnerId não pode ser nulo ou vazio.", nameof(ownerId));

            return await _context.Investments
                .AsNoTracking()
                .LongCountAsync(e => e.OwnerId == ownerId);
        }

        public async Task UpdateAsync(Investment investment)
        {
            _context.Investments.Update(investment);
            await _context.SaveChangesAsync();
        }
    }
}