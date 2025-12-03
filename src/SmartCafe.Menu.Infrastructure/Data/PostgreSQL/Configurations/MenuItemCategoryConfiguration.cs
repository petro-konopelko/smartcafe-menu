using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL.Configurations;

public class MenuItemCategoryConfiguration : IEntityTypeConfiguration<MenuItemCategory>
{
    public void Configure(EntityTypeBuilder<MenuItemCategory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => new { e.MenuItemId, e.CategoryId });

        builder.HasOne(e => e.MenuItem)
            .WithMany(e => e.MenuItemCategories)
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Category)
            .WithMany(e => e.MenuItemCategories)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
