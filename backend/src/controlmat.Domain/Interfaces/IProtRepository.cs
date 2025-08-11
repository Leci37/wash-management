using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces
{
    public interface IProtRepository
    {
        Task<IEnumerable<Prot>> GetByWashingIdAsync(long washingId);
        Task AddAsync(Prot prot);
        Task AddRangeAsync(IEnumerable<Prot> prots);
    }
}
