using Microsoft.EntityFrameworkCore;
using controlmat.Domain.Entities;
using controlmat.Infrastructure.Configurations;

namespace controlmat.Infrastructure.Persistence;

public class ControlmatDbContext : DbContext
{
    public ControlmatDbContext(DbContextOptions<ControlmatDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Machine> Machines => Set<Machine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired();
        });

        modelBuilder.ApplyConfiguration(new MachineConfiguration());
    }
}

