using System.Threading.Tasks;
using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
}

