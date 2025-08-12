using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class MachineRepository : IMachineRepository
{
    private readonly SumisanDbContext _context;
    public MachineRepository(SumisanDbContext context) => _context = context;
}

