using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class MachineRepository : IMachineRepository
    {
        private readonly ControlmatDbContext _context;

        public MachineRepository(ControlmatDbContext context)
        {
            _context = context;
        }

        public async Task<Machine?> GetByIdAsync(int id)
        {
            return await _context.Machines.FindAsync(id);
        }

        public async Task<IEnumerable<Machine>> GetAllAsync()
        {
            return await _context.Machines.ToListAsync();
        }
    }
}
