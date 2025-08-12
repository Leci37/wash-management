using System.Collections.Generic;

namespace controlmat.Domain.Entities;

public class Washing
{
    public long WashingId { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
