using Controlmat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Controlmat.Infrastructure.Configurations;

public class ProtConfiguration : IEntityTypeConfiguration<Prot>
{
    public void Configure(EntityTypeBuilder<Prot> builder)
    {
        builder.ToTable("PROTS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProtId).IsRequired().HasMaxLength(7);
        builder.Property(x => x.BatchNumber).IsRequired().HasMaxLength(4);
        builder.Property(x => x.BagNumber).IsRequired().HasMaxLength(5);

        builder.HasOne(x => x.Washing)
            .WithMany(x => x.Prots)
            .HasForeignKey(x => x.WashingId);
    }
}
