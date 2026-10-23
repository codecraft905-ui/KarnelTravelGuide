using Microsoft.EntityFrameworkCore;

namespace KarnelTravelGuide.Models.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TourPackageDb> TourPackages { get; set; }      // existing table
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new ContactMessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserAccountConfiguration());
    }
}
