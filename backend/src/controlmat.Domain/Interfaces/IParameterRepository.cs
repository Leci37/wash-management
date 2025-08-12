using System.Threading.Tasks;
using Controlmat.Domain.Entities;

namespace Controlmat.Domain.Interfaces;

public interface IParameterRepository
{
    Task<Parameter?> GetByNameAsync(string name);
    Task<string?> GetValueAsync(string name);
}
