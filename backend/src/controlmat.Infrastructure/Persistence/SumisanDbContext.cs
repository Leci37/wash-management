using System;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Entities;

namespace Controlmat.Infrastructure.Persistence
{
    public class SumisanDbContext : DbContext
    {
        public SumisanDbContext(DbContextOptions<SumisanDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<Washing> Washings => Set<Washing>();
        public DbSet<Prot> Prots => Set<Prot>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Parameter> Parameters => Set<Parameter>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnType("smallint");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Washing>(entity =>
            {
                entity.HasKey(e => e.WashingId);
                entity.Property(e => e.WashingId).ValueGeneratedNever();

                entity.Property(e => e.MachineId)
                    .HasColumnType("smallint");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("char(1)")
                    .HasDefaultValue('P');

                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.StartObservation)
                    .HasMaxLength(100);

                entity.Property(e => e.FinishObservation)
                    .HasMaxLength(100);

                entity.HasOne(w => w.Machine)
                    .WithMany(m => m.Washings)
                    .HasForeignKey(w => w.MachineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.StartUser)
                    .WithMany(u => u.StartedWashes)
                    .HasForeignKey(w => w.StartUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.EndUser)
                    .WithMany(u => u.FinishedWashes)
                    .HasForeignKey(w => w.EndUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(w => w.Status);
                entity.HasIndex(w => w.MachineId);
                entity.HasIndex(w => w.StartDate);
            });

            modelBuilder.Entity<Prot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ProtId)
                    .IsRequired()
                    .HasMaxLength(7);

                entity.Property(e => e.BatchNumber)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.BagNumber)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(p => p.Washing)
                    .WithMany(w => w.Prots)
                    .HasForeignKey(p => p.WashingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.WashingId);
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(p => p.Washing)
                    .WithMany(w => w.Photos)
                    .HasForeignKey(p => p.WashingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.WashingId);
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(p => p.Name).IsUnique();
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parameter>().HasData(
                new Parameter { Id = 1, Name = "ImagePath", Value = "C:\\SumiSan\\Photos" },
                new Parameter { Id = 2, Name = "MaxPhotosPerWash", Value = "99" },
                new Parameter { Id = 3, Name = "MaxFileSizeMB", Value = "5" },
                new Parameter { Id = 4, Name = "AllowedExtensions", Value = "jpg,jpeg,png" }
            );

            modelBuilder.Entity<Machine>().HasData(
                new Machine { Id = (short)1, Name = "Máquina 1" },
                new Machine { Id = (short)2, Name = "Máquina 2" }
            );

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                modelBuilder.Entity<User>().HasData(
                    new User { UserId = 1, UserName = "operario1", IsActive = true },
                    new User { UserId = 2, UserName = "operario2", IsActive = true },
                    new User { UserId = 3, UserName = "supervisor", IsActive = true }
                );
            }
        }
    }
}
