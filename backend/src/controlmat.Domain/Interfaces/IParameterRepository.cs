using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IParameterRepository
{
    Task<Parameter?> GetByKeyAsync(string key, CancellationToken ct = default);
}
