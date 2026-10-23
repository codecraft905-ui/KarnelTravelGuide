using KarnelTravelGuide.Data;
using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Services;

public class CatalogService : ICatalogService
{
    static readonly string[] North = { "Islamabad", "Murree", "Skardu", "Hunza", "Kaghan", "Naran", "Swat" };
    static readonly string[] Major = { "Karachi", "Lahore", "Islamabad" };

    public IReadOnlyList<Item> All => CatalogData.All;
    public IReadOnlyList<TourPackage> Packages => CatalogData.Packages;
    public IReadOnlyDictionary<string, string> Titles => CatalogData.Titles;
    public IReadOnlyList<Slide> Slides => CatalogData.Slides;

    public Item Find(string name) =>
        string.IsNullOrWhiteSpace(name) ? null : All.FirstOrDefault(i => string.Equals(i.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));

    public TourPackage FindPackage(string name) =>
        string.IsNullOrWhiteSpace(name) ? null : Packages.FirstOrDefault(p => string.Equals(p.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));

    public TourPackage FirstPackageWith(string placeName)
    {
        var item = Find(placeName);
        return item == null ? null : PackagesOf(item).FirstOrDefault();
    }

    // Hotels and resorts are chosen as the "stay" of a package
    public bool IsStay(Item i) => i.Cat == "hotels" || i.Cat == "resorts";

    List<Item> Resolve(string[] names) => names.Select(Find).Where(i => i != null).ToList();
    public List<Item> SpotsOf(TourPackage p) => Resolve(p.Spots);
    public List<Item> StaysOf(TourPackage p) => Resolve(p.Stays);
    public List<Item> RestaurantsOf(TourPackage p) => Resolve(p.Restaurants);
    public List<Item> TravelOf(TourPackage p) => Resolve(p.Travel);

    // Packages that include this place (used to send a visitor from a card to the package)
    public List<TourPackage> PackagesOf(Item i) =>
        Packages.Where(p => new[] { p.Spots, p.Stays, p.Restaurants, p.Travel }.Any(l => l.Contains(i.Name, StringComparer.OrdinalIgnoreCase))).ToList();
    public bool InAnyPackage(Item i) => PackagesOf(i).Count > 0;

    // Plain-text summary stored with a booking
    public string Summary(TourPackage p) =>
        "Hotel / Resort: " + string.Join(", ", p.Stays) + "\nRestaurants: " + string.Join(", ", p.Restaurants) +
        "\nTourist spots: " + string.Join(", ", p.Spots) + "\nTravel: " + string.Join(", ", p.Travel);

    // Restaurants in the same city as a hotel / resort
    public List<Item> RestaurantsIn(string city) =>
        All.Where(i => i.Cat == "restaurants" && string.Equals(i.City, city, StringComparison.OrdinalIgnoreCase)).OrderByDescending(i => i.Rating).ToList();

    // Transport that works for a place: city-specific routes + nationwide / major-city / northern services
    public List<Item> TravelFor(string city) =>
        All.Where(i => i.Cat == "travel" && (
                i.City.Equals("Nationwide", StringComparison.OrdinalIgnoreCase)
                || i.City.Contains(city, StringComparison.OrdinalIgnoreCase)
                || (i.City == "Major cities" && Major.Contains(city, StringComparer.OrdinalIgnoreCase))
                || (i.City == "Northern Areas" && North.Contains(city, StringComparer.OrdinalIgnoreCase))))
           .OrderByDescending(i => i.Rating).ToList();

    // Shared search used by both the simple and the advanced search pages
    public IEnumerable<Item> Query(string q, string cat, string city, int? min, int? max, double? rating, int? qty, bool avail, string sort)
    {
        var r = All.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q)) r = r.Where(i => (i.Name + " " + i.Desc + " " + i.City).Contains(q.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(cat)) r = r.Where(i => i.Cat == cat);
        if (!string.IsNullOrWhiteSpace(city)) r = r.Where(i => i.City.Contains(city.Trim(), StringComparison.OrdinalIgnoreCase));
        if (min.HasValue) r = r.Where(i => i.Price >= min);
        if (max.HasValue) r = r.Where(i => i.Price <= max);
        if (rating.HasValue) r = r.Where(i => i.Rating >= rating);
        if (qty.HasValue) r = r.Where(i => i.Qty >= qty);
        if (avail) r = r.Where(i => i.Avail);
        return sort switch
        {
            "price-asc" => r.OrderBy(i => i.Price), "price-desc" => r.OrderByDescending(i => i.Price),
            "rating" => r.OrderByDescending(i => i.Rating), _ => r.OrderBy(i => i.Name)
        };
    }

    // Called at start-up: a typo in a package list stops the site with a clear message instead of silently dropping a place
    public void Validate()
    {
        var bad = Packages.SelectMany(p => new[] { p.Spots, p.Stays, p.Restaurants, p.Travel }.SelectMany(l => l).Where(n => Find(n) == null).Select(n => $"{p.Name}: \"{n}\"")).ToList();
        if (bad.Count > 0) throw new InvalidOperationException("Package lists in CatalogData.cs name places that do not exist: " + string.Join("; ", bad));
    }
}
