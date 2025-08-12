using Controlmat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace controlmat.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USERS");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedOnAdd();
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
    }
}
