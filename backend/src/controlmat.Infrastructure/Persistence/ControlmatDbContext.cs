using Microsoft.EntityFrameworkCore;
using controlmat.Domain.Entities;

namespace controlmat.Infrastructure.Persistence;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlmatDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

