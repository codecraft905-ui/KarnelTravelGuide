using KarnelTravelGuide.Data;
using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Services;

// Top menu and home tiles
public class NavigationService
{
    static NavLink L(string t, string u) => new(t, u);
    static NavLink Pkg(TourPackage p) => new(p.Name, "/Packages/Details?name=" + Uri.EscapeDataString(p.Name));

    public NavItem[] Menu { get; }
    public IReadOnlyList<(string Href, string Title, string Kw)> Tiles => CatalogData.Tiles;

    public NavigationService(ICatalogService catalog)
    {
        // Tourist spots, hotels, resorts, restaurants and transport are NOT separate menu items:
        // they are all managed inside the Packages (Packages -> pick a package -> its spots, hotels, restaurants, travel).
        Menu = new NavItem[]
        {
            new("Home", "/", new NavLink[0]),
            new("About Us", "/About", new[]{ L("Our Story","/About#story"), L("Our Facilities","/About#facilities"), L("Our Team","/About#team") }),
            new("Packages", "/Packages", catalog.Packages.Select(Pkg).Append(L("All Packages","/Packages")).ToArray()),
            new("Contact Us", "/Contact", new[]{ L("Book a Tour","/Book"), L("My Booking","/MyBooking"), L("Send Feedback","/Feedback"), L("Our Branches","/Contact#branches"), L("FAQs","/Contact#faq") }),
        };
    }
}
