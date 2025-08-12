using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IMachineRepository
{
    Task<Machine?> GetByIdAsync(int machineId);
    Task<IEnumerable<Machine>> GetAllAsync();
}
