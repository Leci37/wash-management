namespace Controlmat.Domain.Entities
{
    public class Prot
    {
        public int Id { get; set; }
        public long WashingId { get; set; }
        public string ProtId { get; set; } = string.Empty; // Format: PROTXXX
        public string BatchNumber { get; set; } = string.Empty; // Format: NLXX
        public string BagNumber { get; set; } = string.Empty; // Format: XX/XX

        // Navigation properties
        public virtual Washing Washing { get; set; } = default!;
    }
}
