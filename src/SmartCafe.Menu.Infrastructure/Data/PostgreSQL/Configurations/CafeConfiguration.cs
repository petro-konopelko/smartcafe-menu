using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class CafeConfiguration : IEntityTypeConfiguration<Domain.Entities.Cafe>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Cafe> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Generated via Guid.CreateVersion7() in code

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ContactInfo)
            .HasMaxLength(500);

        builder.HasIndex(e => e.Name);

        // Ignore domain events collection (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
