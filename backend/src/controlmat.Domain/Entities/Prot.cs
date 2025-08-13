namespace Controlmat.Domain.Entities;

public class Prot
{
    public int Id { get; set; }
    public long WashingId { get; set; }
    public string ProtId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string BagNumber { get; set; } = string.Empty;

    public Washing Washing { get; set; } = default!;
}
