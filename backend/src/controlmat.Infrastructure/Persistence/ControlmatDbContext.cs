using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;

namespace Controlmat.Infrastructure.Persistence
{
    public class ControlmatDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<Washing> Washings => Set<Washing>();
        public DbSet<Prot> Prots => Set<Prot>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Parameter> Parameters => Set<Parameter>();

        public ControlmatDbContext(DbContextOptions<ControlmatDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
            });

            // Machines
            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            });

            // Washings
            modelBuilder.Entity<Washing>(entity =>
            {
                entity.HasKey(e => e.WashingId);
                entity.Property(e => e.Status).HasColumnType("char(1)").IsRequired();
                entity.Property(e => e.StartObservation).HasMaxLength(100);
                entity.Property(e => e.FinishObservation).HasMaxLength(100);

                // Foreign Keys
                entity.HasOne(w => w.Machine)
                    .WithMany(m => m.Washings)
                    .HasForeignKey(w => w.MachineId);

                entity.HasOne(w => w.StartUser)
                    .WithMany(u => u.StartedWashings)
                    .HasForeignKey(w => w.StartUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.EndUser)
                    .WithMany(u => u.FinishedWashings)
                    .HasForeignKey(w => w.EndUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Prots
            modelBuilder.Entity<Prot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProtId).HasMaxLength(7).IsRequired();
                entity.Property(e => e.BatchNumber).HasMaxLength(4).IsRequired();
                entity.Property(e => e.BagNumber).HasMaxLength(5).IsRequired();

                entity.HasOne(p => p.Washing)
                    .WithMany(w => w.Prots)
                    .HasForeignKey(p => p.WashingId);
            });

            // Photos
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FilePath).HasMaxLength(255).IsRequired();

                entity.HasOne(p => p.Washing)
                    .WithMany(w => w.Photos)
                    .HasForeignKey(p => p.WashingId);
            });

            // Parameters
            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Value).HasMaxLength(255).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
