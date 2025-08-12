using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IMachineRepository
{
    Task<List<Machine>> GetAllAsync();
}
