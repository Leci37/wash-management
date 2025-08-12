using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class WashingRepository : IWashingRepository
{
    private readonly SumisanDbContext _context;
    public WashingRepository(SumisanDbContext context) => _context = context;
}

