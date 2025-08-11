using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces
{
    public interface IWashingRepository
    {
        Task<Washing?> GetByIdAsync(long id);
        Task<IEnumerable<Washing>> GetActiveAsync();
        Task<int> CountActiveAsync();
        Task<bool> IsMachineInUseAsync(int machineId);
        Task AddAsync(Washing washing);
        Task UpdateAsync(Washing washing);
        Task<long> GetNextWashingIdAsync();
    }
}
