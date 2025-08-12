using System.Collections.Generic;

namespace Controlmat.Domain.Entities;

public class Machine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Washing> Washings { get; set; } = new List<Washing>();
}

