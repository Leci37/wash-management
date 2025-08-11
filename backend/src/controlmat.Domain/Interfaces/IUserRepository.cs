using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUserNameAsync(string userName);
        Task<IEnumerable<User>> GetAllActiveAsync();
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task UpdateLastLoginAsync(int userId);
    }
}
