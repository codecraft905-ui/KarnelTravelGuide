using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

// One package with everything that belongs to it
public partial class PackageViewModel
{
    public TourPackage Package { get; set; }
    public List<Item> Spots { get; set; } = new();
    public List<Item> Stays { get; set; } = new();        // hotels + resorts
    public List<Item> Restaurants { get; set; } = new();
    public List<Item> Travel { get; set; } = new();
}
