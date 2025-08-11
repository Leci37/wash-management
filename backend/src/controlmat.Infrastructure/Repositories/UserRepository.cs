using Microsoft.EntityFrameworkCore;
using controlmat.Domain.Entities;
using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ControlmatDbContext _context;

    public UserRepository(ControlmatDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
    }
}

