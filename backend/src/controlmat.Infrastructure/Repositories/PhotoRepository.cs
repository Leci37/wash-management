using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly SumisanDbContext _context;

        public PhotoRepository(SumisanDbContext context)
        {
            _context = context;
        }

        public async Task<List<Photo>> GetByWashIdAsync(long washingId)
        {
            return await _context.Photos
                .Where(p => p.WashingId == washingId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountByWashingIdAsync(long washingId)
        {
            return await _context.Photos.CountAsync(p => p.WashingId == washingId);
        }

        public async Task AddAsync(Photo photo)
        {
            await _context.Photos.AddAsync(photo);
            await _context.SaveChangesAsync();
        }
    }
}
