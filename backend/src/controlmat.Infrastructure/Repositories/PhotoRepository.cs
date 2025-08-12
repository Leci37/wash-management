using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly SumisanDbContext _context;
    public PhotoRepository(SumisanDbContext context) => _context = context;
}

