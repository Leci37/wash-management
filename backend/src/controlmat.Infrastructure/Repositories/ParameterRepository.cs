using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories
{
    public class ParameterRepository : IParameterRepository
    {
        private readonly SumisanDbContext _context;

        public ParameterRepository(SumisanDbContext context)
        {
            _context = context;
        }

        public async Task<Parameter?> GetByNameAsync(string name)
        {
            return await _context.Parameters.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<string?> GetValueAsync(string name)
        {
            var parameter = await GetByNameAsync(name);
            return parameter?.Value;
        }
    }
}
