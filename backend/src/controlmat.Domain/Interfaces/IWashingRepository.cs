using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IWashingRepository
{
    Task<List<Washing>> GetActiveWashesAsync();
    Task<Washing?> GetByIdWithDetailsAsync(long washingId);
    Task<bool> IsMachineInUseAsync(long machineId);
}
