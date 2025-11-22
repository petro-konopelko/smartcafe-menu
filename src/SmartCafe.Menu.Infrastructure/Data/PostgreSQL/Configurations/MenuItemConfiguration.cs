using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Price)
            .HasPrecision(10, 2);

        builder.Property(e => e.ImageBigUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.ImageCroppedUrl)
            .HasMaxLength(1000);

        // Store ingredient options as JSONB
        builder.Property(e => e.IngredientOptions)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Domain.ValueObjects.Ingredient>>(v, (JsonSerializerOptions?)null) ?? new List<Domain.ValueObjects.Ingredient>());

        builder.HasOne(e => e.Section)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.SectionId);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.IsActive);

        // JSONB index for ingredient queries
        builder.HasIndex(e => e.IngredientOptions)
            .HasMethod("gin");

        // Check constraint for positive price
        builder.ToTable(t => t.HasCheckConstraint("CK_MenuItems_Price_Positive", "\"Price\" > 0"));
    }
}
