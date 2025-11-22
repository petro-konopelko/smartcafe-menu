using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.IconUrl)
            .HasMaxLength(1000);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.IsDefault);
    }
}
