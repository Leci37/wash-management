
using System.Collections.Generic;
using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(int photoId);
    Task<List<Photo>> GetByWashIdAsync(long washingId);
    Task<int> CountByWashingIdAsync(long washingId);
    Task AddAsync(Photo photo);
}
