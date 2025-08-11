namespace Controlmat.Domain.Entities;

public class Machine
{
    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
}
