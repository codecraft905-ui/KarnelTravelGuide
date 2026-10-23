using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarnelTravelGuide.Models.Data;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> e)
    {
        e.ToTable("ContactMessages");
        e.HasKey(m => m.Id);
        e.Property(m => m.Id).HasMaxLength(32);
        e.Property(m => m.Name).IsRequired().HasMaxLength(100);
        e.Property(m => m.Email).IsRequired().HasMaxLength(150);
        e.Property(m => m.Phone).HasMaxLength(30);
        e.Property(m => m.Type).IsRequired().HasMaxLength(20);
        e.Property(m => m.Text).IsRequired().HasMaxLength(1000);
        e.Property(m => m.Destination).HasMaxLength(200);
        e.HasIndex(m => m.CreatedAtUtc);
    }
}
