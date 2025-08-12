using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
}
