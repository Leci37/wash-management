
using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsAsync(int userId);

}
