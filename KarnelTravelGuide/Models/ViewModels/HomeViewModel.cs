using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

public partial class HomeViewModel
{
    public IReadOnlyList<Slide> Slides { get; set; } = new List<Slide>();
    public List<Item> TopSpots { get; set; } = new();
    public List<TourPackage> FeaturedPackages { get; set; } = new();
    public List<Item> FeaturedStays { get; set; } = new();
    public List<Item> Restaurants { get; set; } = new();
    public List<Item> Transport { get; set; } = new();
}

public partial class InformationViewModel
{
    public List<Item> NewHotels { get; set; } = new();
    public List<Item> NewRestaurants { get; set; } = new();
    public List<Item> NewResorts { get; set; } = new();
    public List<TourPackage> Packages { get; set; } = new();
}
