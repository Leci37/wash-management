namespace Controlmat.Domain.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public long WashingId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Washing Washing { get; set; } = default!;
    }
}
