using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces
{
    public interface IMachineRepository
    {
        Task<Machine?> GetByIdAsync(int id);
        Task<IEnumerable<Machine>> GetAllAsync();
    }
}
