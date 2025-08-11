namespace Controlmat.Domain.Entities
{
    public class Washing
    {
        public long WashingId { get; set; }
        public int MachineId { get; set; }
        public int StartUserId { get; set; }
        public int? EndUserId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public char Status { get; set; } = 'P'; // 'P' = Progress, 'F' = Finished
        public string? StartObservation { get; set; }
        public string? FinishObservation { get; set; }

        // Navigation properties
        public virtual Machine Machine { get; set; } = default!;
        public virtual User StartUser { get; set; } = default!;
        public virtual User? EndUser { get; set; }
        public virtual ICollection<Prot> Prots { get; set; } = new List<Prot>();
        public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

        // Business methods
        public static long GenerateWashingId()
        {
            var now = DateTime.Now;
            var dateStr = now.ToString("yyMMdd");
            var sequence = 1; // TODO: Get from database sequence
            return long.Parse($"{dateStr}{sequence:D2}");
        }
    }
}
