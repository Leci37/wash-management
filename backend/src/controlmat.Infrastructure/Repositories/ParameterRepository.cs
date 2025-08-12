using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class ParameterRepository : IParameterRepository
{
    private readonly SumisanDbContext _context;
    public ParameterRepository(SumisanDbContext context) => _context = context;
}

