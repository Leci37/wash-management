using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SumisanDbContext _context;
    public UserRepository(SumisanDbContext context) => _context = context;
}

