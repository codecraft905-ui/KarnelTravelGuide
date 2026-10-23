namespace KarnelTravelGuide.Models;

// One record type for spots, hotels, restaurants, resorts and transport
public record Item(string Cat, string Name, string City, int Price, double Rating, int Qty, bool Avail, string Desc, string Kw);
