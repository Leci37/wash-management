using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IProtRepository
{
    Task AddAsync(Prot prot, CancellationToken ct = default);
}
