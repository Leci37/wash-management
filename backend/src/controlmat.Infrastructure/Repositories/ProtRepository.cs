using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class ProtRepository : IProtRepository
    {
        private readonly ControlmatDbContext _context;

        public ProtRepository(ControlmatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prot>> GetByWashingIdAsync(long washingId)
        {
            return await _context.Prots
                .Where(p => p.WashingId == washingId)
                .ToListAsync();
        }

        public async Task AddAsync(Prot prot)
        {
            _context.Prots.Add(prot);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Prot> prots)
        {
            _context.Prots.AddRange(prots);
            await _context.SaveChangesAsync();
        }
    }
}
