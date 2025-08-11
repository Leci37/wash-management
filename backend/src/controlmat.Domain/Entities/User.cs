namespace Controlmat.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "WarehouseUser";

        // Navigation properties
        public virtual ICollection<Washing> StartedWashings { get; set; } = new List<Washing>();
        public virtual ICollection<Washing> FinishedWashings { get; set; } = new List<Washing>();
    }
}
