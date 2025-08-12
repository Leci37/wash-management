using controlmat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace controlmat.Infrastructure.Configurations;

public class WashingConfiguration : IEntityTypeConfiguration<Washing>
{
    public void Configure(EntityTypeBuilder<Washing> builder)
    {
        builder.ToTable("WASHINGS");
        builder.HasKey(x => x.WashingId);
        builder.Property(x => x.WashingId).ValueGeneratedNever();
        builder.Property(x => x.Status).IsRequired().HasMaxLength(1).IsFixedLength();
        builder.Property(x => x.StartObservation).HasMaxLength(100);
        builder.Property(x => x.FinishObservation).HasMaxLength(100);

        builder.HasOne(x => x.Machine).WithMany().HasForeignKey(x => x.MachineId);
        builder.HasOne(x => x.StartUser).WithMany().HasForeignKey(x => x.StartUserId);
        builder.HasOne(x => x.EndUser).WithMany().HasForeignKey(x => x.EndUserId);
    }
}
