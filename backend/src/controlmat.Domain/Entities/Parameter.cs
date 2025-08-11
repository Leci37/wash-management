namespace Controlmat.Domain.Entities;

public class Parameter
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
}
