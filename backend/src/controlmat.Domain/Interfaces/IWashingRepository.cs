
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;


public interface IWashingRepository
{
    Task<Washing?> GetByIdAsync(long id);
    Task<Washing?> GetByIdWithDetailsAsync(long id);
    Task<List<Washing>> GetActiveWashesAsync();
    /// <summary>
    /// Retrieves the currently active wash for the specified machine, if any.
    /// </summary>
    /// <param name="machineId">Identifier of the machine.</param>
    /// <returns>The active <see cref="Washing"/> instance or <c>null</c> if none exists.</returns>
    Task<Washing?> GetActiveWashByMachineAsync(int machineId);
    Task<int> CountActiveAsync();
    Task<bool> IsMachineInUseAsync(short machineId);
    Task<long?> GetMaxWashingIdByDateAsync(DateTime date);
    Task AddAsync(Washing washing);
    Task UpdateAsync(Washing washing);

}
