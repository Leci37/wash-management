using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class ProtRepository : IProtRepository
{
    private readonly SumisanDbContext _context;
    public ProtRepository(SumisanDbContext context) => _context = context;
}

