using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IWashingRepository
{
    Task<Washing?> GetByIdAsync(string washingId, CancellationToken ct = default);
    Task<IReadOnlyList<Washing>> GetActiveAsync(int take, CancellationToken ct = default);
    Task AddAsync(Washing washing, CancellationToken ct = default);
    Task UpdateAsync(Washing washing, CancellationToken ct = default);
}
