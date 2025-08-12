using System;

namespace controlmat.Domain.Entities;

public class Photo
{
    public int Id { get; set; }
    public long WashingId { get; set; }
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public Washing Washing { get; set; } = default!;
}
