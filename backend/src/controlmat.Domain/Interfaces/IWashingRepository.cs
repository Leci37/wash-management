using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IWashingRepository
{
    Task<Washing?> GetByIdAsync(long id);
}
