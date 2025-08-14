
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
    Task<Washing?> GetActiveWashByMachineAsync(int machineId);
    Task<int> CountActiveAsync();
    Task<bool> IsMachineInUseAsync(int machineId);
    Task<List<Washing>> GetActiveWashesByMachineAsync(int machineId);
    Task<long?> GetMaxWashingIdByDateAsync(DateTime date);
    /// <summary>
    /// Gets all WashingIds that start with the given date prefix (YYMMDD)
    /// Used for generating sequential WashingIds within a day
    /// </summary>
    /// <param name="datePrefix">Date prefix in YYMMDD format (e.g., "250813")</param>
    /// <returns>List of WashingIds matching the date prefix</returns>
    Task<IEnumerable<long>> GetWashingIdsByDatePrefixAsync(string datePrefix);
    Task AddAsync(Washing washing);
    Task UpdateAsync(Washing washing);

}
