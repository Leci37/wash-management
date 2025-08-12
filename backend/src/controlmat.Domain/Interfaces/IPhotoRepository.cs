using controlmat.Domain.Entities;

namespace controlmat.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<int> CountByWashingIdAsync(long washingId);
    Task AddAsync(Photo photo);
}
