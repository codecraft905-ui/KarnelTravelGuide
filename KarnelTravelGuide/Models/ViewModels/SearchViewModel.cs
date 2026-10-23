using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

// Shared by the Search and Advanced Search pages (values come from the query string)
public partial class SearchViewModel
{
    public string Q { get; set; }
    public string Cat { get; set; }
    public string City { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public double? Rating { get; set; }
    public int? Qty { get; set; }
    public bool Avail { get; set; }
    public string Sort { get; set; }
    public IEnumerable<Item> Results { get; set; } = Enumerable.Empty<Item>();
    public Item Highlight { get; set; }      // the place the user searched for (e.g. "Hunza") - its name and photo go in the banner
}
