namespace Controlmat.Domain.Entities
{
    public class Machine
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Washing> Washings { get; set; } = new List<Washing>();
    }
}
