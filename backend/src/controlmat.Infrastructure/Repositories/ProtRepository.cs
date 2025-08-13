using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class ProtRepository : IProtRepository
    {
        private readonly SumisanDbContext _context;

        public ProtRepository(SumisanDbContext context)
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
            await _context.Prots.AddAsync(prot);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetActiveProtIdsAsync()
        {
            return await _context.Prots
                .Where(p => p.Washing.Status == 'P')
                .Select(p => p.ProtId)
                .ToListAsync();
        }
    }
}
