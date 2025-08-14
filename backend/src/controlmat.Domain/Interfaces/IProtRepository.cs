using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IProtRepository
{
    Task<IEnumerable<Prot>> GetByWashingIdAsync(long washingId);
    Task AddAsync(Prot prot);
    Task<bool> ExistsInWashAsync(long washingId, string protId, string batchNumber, string bagNumber);
}
