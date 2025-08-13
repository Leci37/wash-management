
using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(int photoId);
    Task<List<Photo>> GetByWashingIdAsync(long washingId);
    Task AddAsync(Photo photo);
    Task<int> CountByWashingIdAsync(long washingId);
}
