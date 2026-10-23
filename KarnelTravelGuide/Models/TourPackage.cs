namespace KarnelTravelGuide.Models;

// A COMPLETE tour package: the stay, food, places to visit and transport are already included.
// Spots / Stays / Restaurants / Travel are item NAMES from CatalogData.All (checked at start-up).
public record TourPackage(string Name, string Offer, string Kw, string[] Spots, string[] Stays, string[] Restaurants, string[] Travel);
