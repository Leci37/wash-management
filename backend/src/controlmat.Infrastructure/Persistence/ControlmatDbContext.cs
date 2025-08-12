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

    public DbSet<Prot> Prots => Set<Prot>();

    public DbSet<Washing> Washings => Set<Washing>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        modelBuilder.ApplyConfiguration(new ProtConfiguration());

        modelBuilder.ApplyConfiguration(new WashingConfiguration());

    }
}

