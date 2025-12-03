using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Domain.Entities.Menu>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Menu> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Generated via Guid.CreateVersion7() in code

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.State)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(e => e.Cafe)
            .WithMany()
            .HasForeignKey(e => e.CafeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Sections)
            .WithOne(e => e.Menu)
            .HasForeignKey(e => e.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for common query patterns
        builder.HasIndex(e => new { e.CafeId, e.CreatedAt });
        builder.HasIndex(e => new { e.CafeId, e.State });

        // Unique partial index: Only one active menu per cafe
        builder.HasIndex(e => e.CafeId)
            .IsUnique()
            .HasFilter($"\"State\" = {(int)MenuState.Active}")
            .HasDatabaseName("UX_Menus_CafeId_Active");

        // Global query filter for soft delete
        builder.HasQueryFilter(e => e.State != MenuState.Deleted);

        // Ignore domain events collection (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
