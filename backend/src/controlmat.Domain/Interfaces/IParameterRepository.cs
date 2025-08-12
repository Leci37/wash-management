namespace controlmat.Domain.Interfaces;

public interface IParameterRepository
{
    Task<string?> GetValueAsync(string name);
}
