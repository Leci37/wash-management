namespace Controlmat.Domain.Entities;

public class Photo
{
    public int Id { get; private set; }
    public string WashingId { get; private set; } = null!;
    public string RelativePath { get; private set; } = null!; // e.g. {Year}/{WashingId}_{XX}.jpg
    public int Sequence { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public static Photo Create(string washingId, string relativePath, int sequence, DateTime uploadedAt)
        => new Photo { WashingId = washingId, RelativePath = relativePath, Sequence = sequence, UploadedAt = uploadedAt };
}
