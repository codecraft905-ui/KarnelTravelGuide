using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarnelTravelGuide.Models.Data;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> e)
    {
        e.ToTable("Bookings");
        e.HasKey(b => b.Id);
        e.Property(b => b.Id).HasMaxLength(20);
        e.Property(b => b.AccountEmail).IsRequired().HasMaxLength(150);
        e.Property(b => b.Name).IsRequired().HasMaxLength(100);
        e.Property(b => b.Email).IsRequired().HasMaxLength(150);
        e.Property(b => b.Phone).HasMaxLength(30);
        e.Property(b => b.Destination).HasMaxLength(200);
        e.Property(b => b.Kind).HasMaxLength(50);
        e.Property(b => b.Location).HasMaxLength(150);
        e.Property(b => b.Hotel).HasMaxLength(200);
        e.Property(b => b.HotelLocation).HasMaxLength(150);
        e.Property(b => b.Included).HasMaxLength(2000);
        e.Property(b => b.Message).HasMaxLength(1000);
        e.Property(b => b.AdminNote).HasMaxLength(500);
        e.Property(b => b.Status).HasConversion<string>().HasMaxLength(20).IsRequired();   // saved as text: Pending / Confirmed / Rejected
        e.HasIndex(b => b.AccountEmail);
        e.HasIndex(b => b.Status);
    }
}
