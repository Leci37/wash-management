
using System.Collections.Generic;
namespace controlmat.Domain.Entities;

public class Washing
{
    public long WashingId { get; set; }

    public short MachineId { get; set; }
    public int StartUserId { get; set; }
    public int? EndUserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = default!; // char(1)
    public string? StartObservation { get; set; }
    public string? FinishObservation { get; set; }

    public Machine Machine { get; set; } = default!;
    public User StartUser { get; set; } = default!;
    public User? EndUser { get; set; }
    public ICollection<Prot> Prots { get; set; } = new List<Prot>();

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
