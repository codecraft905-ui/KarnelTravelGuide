using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarnelTravelGuide.Models.Data;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> e)
    {
        e.ToTable("UserAccounts");
        e.HasKey(u => u.Id);
        e.Property(u => u.Id).HasMaxLength(32);
        e.Property(u => u.Name).IsRequired().HasMaxLength(100);
        e.Property(u => u.Email).IsRequired().HasMaxLength(150);
        e.Property(u => u.Phone).HasMaxLength(30);
        e.Property(u => u.Salt).IsRequired().HasMaxLength(64);
        e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
        e.HasIndex(u => u.Email).IsUnique();     // one account per email
    }
}
