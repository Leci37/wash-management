using System.Collections.Generic;

namespace controlmat.Domain.Entities;

public class Machine
{
    public short Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<Washing> Washings { get; set; } = new List<Washing>();
}

