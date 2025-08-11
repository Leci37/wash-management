namespace Controlmat.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "WarehouseUser";
        public bool? IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        // Navigation properties
        public virtual ICollection<Washing> StartedWashings { get; set; } = new List<Washing>();
        public virtual ICollection<Washing> FinishedWashings { get; set; } = new List<Washing>();

        // Business methods
        public bool CanManageWashes() => (IsActive ?? true) && (Role == "WarehouseUser" || Role == "Supervisor");
        public bool CanViewReports() => (IsActive ?? true) && (Role == "Supervisor" || Role == "Administrator");
        public bool IsAdministrator() => (IsActive ?? true) && Role == "Administrator";
    }
}
