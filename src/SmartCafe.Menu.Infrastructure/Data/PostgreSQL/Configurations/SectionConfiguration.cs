using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(Section.MaxNameLength);

        builder.HasOne(e => e.Menu)
            .WithMany(e => e.Sections)
            .HasForeignKey(e => e.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Items collection with backing field
        builder.HasMany(e => e.Items)
            .WithOne(e => e.Section)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Use backing field for Items collection
        builder
            .Navigation(e => e.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(e => e.MenuId);
        builder.HasIndex(e => new { e.MenuId, e.Position });

        // Ignore domain events collection (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
