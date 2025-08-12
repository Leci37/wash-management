using System.Collections.Generic;

namespace controlmat.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string? Role { get; set; }

    public ICollection<Washing> StartedWashes { get; set; } = new List<Washing>();
    public ICollection<Washing> FinishedWashes { get; set; } = new List<Washing>();
}

