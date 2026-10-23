using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Services;

// Read-only access to the site content (places, packages) used by controllers and views
public interface ICatalogService
{
    IReadOnlyList<Item> All { get; }
    IReadOnlyList<TourPackage> Packages { get; }
    IReadOnlyDictionary<string, string> Titles { get; }
    IReadOnlyList<Slide> Slides { get; }

    Item Find(string name);
    TourPackage FindPackage(string name);
    TourPackage FirstPackageWith(string placeName);      // old /Book?item=<place> links

    bool IsStay(Item i);
    List<Item> SpotsOf(TourPackage p);
    List<Item> StaysOf(TourPackage p);
    List<Item> RestaurantsOf(TourPackage p);
    List<Item> TravelOf(TourPackage p);
    List<TourPackage> PackagesOf(Item i);
    bool InAnyPackage(Item i);
    string Summary(TourPackage p);

    List<Item> RestaurantsIn(string city);
    List<Item> TravelFor(string city);
    IEnumerable<Item> Query(string q, string cat, string city, int? min, int? max, double? rating, int? qty, bool avail, string sort);

    void Validate();
}
