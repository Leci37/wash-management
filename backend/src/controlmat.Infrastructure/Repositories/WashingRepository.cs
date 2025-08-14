using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class WashingRepository : IWashingRepository
    {
        private readonly SumisanDbContext _context;

        public WashingRepository(SumisanDbContext context)
        {
            _context = context;
        }

        public async Task<Washing?> GetByIdAsync(long id)
        {
            return await _context.Washings.FindAsync(id);
        }

        public async Task<Washing?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.Washings
                .Include(w => w.Machine)
                .Include(w => w.StartUser)
                .Include(w => w.EndUser)
                .Include(w => w.Prots)
                .Include(w => w.Photos)
                .FirstOrDefaultAsync(w => w.WashingId == id);
        }

        public async Task<List<Washing>> GetActiveWashesAsync()
        {
            return await _context.Washings
                .Where(w => w.Status == 'P')
                .Include(w => w.StartUser)
                .Include(w => w.Machine)
                .ToListAsync();
        }

        public async Task<Washing?> GetActiveWashByMachineAsync(int machineId)
        {
            return await _context.Washings
                .Where(w => w.MachineId == machineId && w.Status == 'P')
                .Include(w => w.Prots)
                .Include(w => w.Photos)
                .Include(w => w.StartUser)
                .Include(w => w.EndUser)
                .Include(w => w.Machine)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountActiveAsync()
        {
            return await _context.Washings.CountAsync(w => w.Status != 'F');
        }

        public async Task<bool> IsMachineInUseAsync(short machineId)
        {
            return await _context.Washings.AnyAsync(w => w.MachineId == machineId && w.Status != 'F');
        }

        public async Task<long?> GetMaxWashingIdByDateAsync(DateTime date)
        {
            return await _context.Washings
                .Where(w => w.StartDate.Date == date.Date)
                .Select(w => (long?)w.WashingId)
                .MaxAsync();
        }

        public async Task AddAsync(Washing washing)
        {
            await _context.Washings.AddAsync(washing);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Washing washing)
        {
            _context.Washings.Update(washing);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all WashingIds that start with the given date prefix (YYMMDD)
        /// Used for generating sequential WashingIds within a day
        /// </summary>
        public async Task<IEnumerable<long>> GetWashingIdsByDatePrefixAsync(string datePrefix)
        {
            if (string.IsNullOrEmpty(datePrefix) || datePrefix.Length != 6)
            {
                throw new ArgumentException("Date prefix must be 6 characters in YYMMDD format", nameof(datePrefix));
            }

            // Parse the date prefix to get the range
            var prefixAsLong = long.Parse(datePrefix) * 100; // e.g., 250813 -> 25081300
            var rangeStart = prefixAsLong;                    // 25081300
            var rangeEnd = prefixAsLong + 99;                 // 25081399

            return await _context.Washings
                .Where(w => w.WashingId >= rangeStart && w.WashingId <= rangeEnd)
                .Select(w => w.WashingId)
                .OrderBy(id => id)
                .ToListAsync();
        }
    }
}
