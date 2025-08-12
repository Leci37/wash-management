namespace controlmat.Domain.Entities;

public class Prot
{
    public int Id { get; set; }
    public long WashingId { get; set; }
    public string ProtId { get; set; } = default!;
    public string BatchNumber { get; set; } = default!;
    public string BagNumber { get; set; } = default!;

    public Washing Washing { get; set; } = default!;
}
