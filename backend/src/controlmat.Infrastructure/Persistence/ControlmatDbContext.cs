using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;
using Controlmat.Infrastructure.Configurations;

namespace Controlmat.Infrastructure.Persistence;

public class ControlmatDbContext : DbContext
{
    public ControlmatDbContext(DbContextOptions<ControlmatDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Washing> Washings => Set<Washing>();
    public DbSet<Prot> Prots => Set<Prot>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Parameter> Parameters => Set<Parameter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ParameterConfiguration());
        modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        modelBuilder.ApplyConfiguration(new ProtConfiguration());
        modelBuilder.ApplyConfiguration(new WashingConfiguration());
    }
}

