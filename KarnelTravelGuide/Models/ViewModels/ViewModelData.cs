using KarnelTravelGuide.Models;
using KarnelTravelGuide.Services;

namespace KarnelTravelGuide.ViewModels;

// Fills the page data from the catalog. The views call these, so the controllers stay empty.

public partial class HomeViewModel
{
    public HomeViewModel Fill(ICatalogService c)
    {
        Slides = c.Slides;
        TopSpots = c.All.Where(x => x.Cat == "tourist-spots" && c.InAnyPackage(x)).OrderByDescending(x => x.Rating).Take(8).ToList();
        FeaturedPackages = c.Packages.Take(3).ToList();
        FeaturedStays = c.All.Where(x => c.IsStay(x) && c.InAnyPackage(x)).OrderByDescending(x => x.Rating).Take(8).ToList();
        Restaurants = c.All.Where(x => x.Cat == "restaurants" && c.InAnyPackage(x)).Take(8).ToList();
        Transport = c.All.Where(x => x.Cat == "travel" && c.InAnyPackage(x)).Take(8).ToList();
        return this;
    }
}

public partial class InformationViewModel
{
    public InformationViewModel Fill(ICatalogService c)
    {
        NewHotels = c.All.Where(x => x.Cat == "hotels").TakeLast(4).ToList();
        NewRestaurants = c.All.Where(x => x.Cat == "restaurants").TakeLast(4).ToList();
        NewResorts = c.All.Where(x => x.Cat == "resorts").TakeLast(4).ToList();
        Packages = c.Packages.Skip(3).ToList();
        return this;
    }
}

public partial class ListingViewModel
{
    public ListingViewModel Fill(ICatalogService c, string cat, string city)
    {
        cat = string.IsNullOrEmpty(cat) ? "tourist-spots" : cat;
        if (cat == "resorts") cat = "hotels";      // resorts live inside the combined Hotels & Resorts page
        Cat = cat;
        Name = cat == "hotels" ? "Hotels & Resorts" : (c.Titles.TryGetValue(cat, out var t) ? t : cat);
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        var items = cat == "hotels" ? c.All.Where(c.IsStay) : c.All.Where(i => i.Cat == cat);
        if (City != null) items = items.Where(i => i.City.Contains(City, StringComparison.OrdinalIgnoreCase));
        Items = items.OrderByDescending(i => i.Rating).ToList();
        return this;
    }
}

public partial class DetailsViewModel
{
    public DetailsViewModel Fill(ICatalogService c, string name)
    {
        var item = c.Find(name);
        if (item == null) return this;
        Item = item;
        if (c.IsStay(item)) { Restaurants = c.RestaurantsIn(item.City); Travel = c.TravelFor(item.City); }
        else Related = c.All.Where(i => i != item && string.Equals(i.City, item.City, StringComparison.OrdinalIgnoreCase)).Take(4).ToList();
        return this;
    }
}

public partial class PackageViewModel
{
    public PackageViewModel Fill(ICatalogService c, string name)
    {
        var p = c.FindPackage(name);
        if (p == null) return this;
        Package = p; Spots = c.SpotsOf(p); Stays = c.StaysOf(p); Restaurants = c.RestaurantsOf(p); Travel = c.TravelOf(p);
        return this;
    }
}

public partial class SearchViewModel
{
    public SearchViewModel Fill(ICatalogService c)
    {
        var results = c.Query(Q, Cat, City, MinPrice, MaxPrice, Rating, Qty, Avail, Sort).ToList();
        Results = results;
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var q = Q.Trim();
            Highlight = results.OrderBy(i => i.Name.Equals(q, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                               .ThenBy(i => i.Name.StartsWith(q, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                               .ThenBy(i => i.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                               .ThenBy(i => i.Cat == "tourist-spots" ? 0 : 1)
                               .FirstOrDefault();
        }
        return this;
    }
}
