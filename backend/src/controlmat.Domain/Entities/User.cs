using System.Collections.Generic;

namespace Controlmat.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public ICollection<Washing> StartedWashes { get; set; } = new List<Washing>();
    public ICollection<Washing> FinishedWashes { get; set; } = new List<Washing>();
}

