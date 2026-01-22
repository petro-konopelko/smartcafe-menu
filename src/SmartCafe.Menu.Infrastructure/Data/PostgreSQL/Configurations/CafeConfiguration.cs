using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class CafeConfiguration : IEntityTypeConfiguration<Cafe>
{
    public void Configure(EntityTypeBuilder<Cafe> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Generated via Guid.CreateVersion7() in code

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(Cafe.MaxNameLength);

        builder.Property(e => e.ContactInfo)
            .HasMaxLength(Cafe.MaxContactInfoLength);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(e => e.Name);

        // Ignore domain events collection (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
