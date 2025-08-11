using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<int> CountForWashingAsync(string washingId, CancellationToken ct = default);
    Task AddAsync(Photo photo, CancellationToken ct = default);
}
