
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;


public interface IWashingRepository
{
    Task<Washing?> GetByIdAsync(long id);
    Task<Washing?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<Washing>> GetActiveWashesAsync();
    Task<Washing?> GetActiveWashByMachineAsync(int machineId);
    Task<int> CountActiveAsync();
    Task<bool> IsMachineInUseAsync(short machineId);
    Task<long?> GetMaxWashingIdByDateAsync(DateTime date);
    Task AddAsync(Washing washing);
    Task UpdateAsync(Washing washing);

}
