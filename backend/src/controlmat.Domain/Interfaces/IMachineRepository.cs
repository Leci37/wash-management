using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IMachineRepository
{
    Task<bool> IsBusyAsync(int machineId, CancellationToken ct = default);
}
