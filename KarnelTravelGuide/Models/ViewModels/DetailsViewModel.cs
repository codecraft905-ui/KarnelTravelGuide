using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

// One place (hotel, resort, spot, restaurant, transport) with what is around it
public partial class DetailsViewModel
{
    public Item Item { get; set; }
    public List<Item> Restaurants { get; set; } = new();   // hotels / resorts: restaurants in the same city
    public List<Item> Travel { get; set; } = new();        // hotels / resorts: transport for that place
    public List<Item> Related { get; set; } = new();       // everything else: more in the same city
}
