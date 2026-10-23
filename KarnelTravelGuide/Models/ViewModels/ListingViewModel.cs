using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

public partial class ListingViewModel
{
    public string Cat { get; set; }
    public string City { get; set; }          // optional: /hotels?city=Karachi
    public List<Item> Items { get; set; } = new();
    public string Name { get; set; }          // "Hotels" also lists the resorts, so it is shown as "Hotels & Resorts"
    public string Title => string.IsNullOrWhiteSpace(City) ? Name : Name + " in " + City;
}
