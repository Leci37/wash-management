using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class MachineRepository : IMachineRepository
    {
        private readonly SumisanDbContext _context;

        public MachineRepository(SumisanDbContext context)
        {
            _context = context;
        }

        public async Task<Machine?> GetByIdAsync(int machineId)
        {
            return await _context.Machines.FindAsync(machineId);
        }

        public async Task<IEnumerable<Machine>> GetAllAsync()
        {
            return await _context.Machines.ToListAsync();
        }
    }
}
