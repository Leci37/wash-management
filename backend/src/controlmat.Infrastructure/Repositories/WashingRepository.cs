using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class WashingRepository : IWashingRepository
    {
        private readonly ControlmatDbContext _context;

        public WashingRepository(ControlmatDbContext context)
        {
            _context = context;
        }

        public async Task<Washing?> GetByIdAsync(long id)
        {
            return await _context.Washings
                .Include(w => w.Machine)
                .Include(w => w.StartUser)
                .Include(w => w.EndUser)
                .Include(w => w.Prots)
                .Include(w => w.Photos)
                .FirstOrDefaultAsync(w => w.WashingId == id);
        }

        public async Task<IEnumerable<Washing>> GetActiveAsync()
        {
            return await _context.Washings
                .Where(w => w.Status == 'P')
                .Include(w => w.Machine)
                .Include(w => w.StartUser)
                .Include(w => w.EndUser)
                .Include(w => w.Prots)
                .Include(w => w.Photos)
                .OrderBy(w => w.StartDate)
                .ToListAsync();
        }

        public async Task<int> CountActiveAsync()
        {
            return await _context.Washings.CountAsync(w => w.Status == 'P');
        }

        public async Task<bool> IsMachineInUseAsync(int machineId)
        {
            return await _context.Washings
                .AnyAsync(w => w.MachineId == machineId && w.Status == 'P');
        }

        public async Task AddAsync(Washing washing)
        {
            _context.Washings.Add(washing);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Washing washing)
        {
            _context.Washings.Update(washing);
            await _context.SaveChangesAsync();
        }

        public async Task<long> GetNextWashingIdAsync()
        {
            var now = DateTime.Now;
            var dateStr = now.ToString("yyMMdd");

            // Get highest sequence for today
            var prefix = long.Parse(dateStr + "00");
            var maxId = await _context.Washings
                .Where(w => w.WashingId >= prefix && w.WashingId < prefix + 100)
                .MaxAsync(w => (long?)w.WashingId) ?? prefix;

            return maxId + 1;
        }
    }
}
