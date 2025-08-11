namespace Controlmat.Domain.Entities;

public class Prot
{
    public int Id { get; private set; }
    public string WashingId { get; private set; } = null!;
    public string ProtCode { get; private set; } = null!;
    public DateTime AddedAt { get; private set; }

    public static Prot Create(string washingId, string protCode, DateTime addedAt)
        => new Prot { WashingId = washingId, ProtCode = protCode, AddedAt = addedAt };
}
