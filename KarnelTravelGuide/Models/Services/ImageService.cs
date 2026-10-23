using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Services;

public interface IImageService
{
    string Img(string kw, int n, int w = 800, int h = 500);
    string Bg(string kw, int n, int w = 1600, int h = 600);
    string LocalPic(Item i);
    string LocalPic(TourPackage p);
    string ItemBg(Item i);
    string PackageBg(TourPackage p);
}

// Finds the picture files in wwwroot/images (or draws a local placeholder when there is none)
public class ImageService : IImageService
{
    readonly string WebRoot;
    public ImageService(IWebHostEnvironment env) { WebRoot = env.WebRootPath; }

    static readonly (string A, string B, string C)[] Themes =
    {
        ("#0f766e", "#14b8a6", "#134e4a"), ("#0f766e", "#f97316", "#7c2d12"), ("#0e7490", "#38bdf8", "#164e63"),
        ("#166534", "#84cc16", "#14532d"), ("#9a3412", "#fbbf24", "#78350f"), ("#1e3a8a", "#38bdf8", "#172554"),
    };

    // Photo for a decorative spot (hero, tiles, gallery, banners ...): wwwroot/images/site/<keyword>.jpg
    // e.g. "mountains,lake" -> mountains-lake.jpg. Optional extra versions: mountains-lake-2.jpg, -3, -4
    // (they are rotated between the places that use the same keyword). Returns null if no photo was added yet.
    string SitePic(string kw, int n)
    {
        if (string.IsNullOrEmpty(WebRoot) || string.IsNullOrEmpty(kw)) return null;
        var slug = Slug(kw);
        var found = new List<string>();
        foreach (var suffix in new[] { "", "-2", "-3", "-4" })
            foreach (var e in Ext)
                if (File.Exists(Path.Combine(WebRoot, "images", "site", slug + suffix + e))) { found.Add("/images/site/" + slug + suffix + e); break; }
        return found.Count == 0 ? null : found[Math.Abs(n) % found.Count];
    }

    public string Img(string kw, int n, int w = 800, int h = 500)
    {
        var photo = SitePic(kw, n);
        if (photo != null) return photo;
        var t = Themes[Math.Abs(n) % Themes.Length];
        var svg = $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {w} {h}' preserveAspectRatio='xMidYMid slice'>" +
                  $"<defs><linearGradient id='g' x1='0' y1='0' x2='1' y2='1'><stop offset='0' stop-color='{t.A}'/><stop offset='1' stop-color='{t.B}'/></linearGradient></defs>" +
                  $"<rect width='{w}' height='{h}' fill='url(#g)'/>" +
                  $"<circle cx='{w * 0.78:0}' cy='{h * 0.26:0}' r='{Math.Min(w, h) * 0.09:0}' fill='#fff' fill-opacity='.35'/>" +
                  $"<polygon points='0,{h} 0,{h * 0.72:0} {w * 0.22:0},{h * 0.5:0} {w * 0.4:0},{h * 0.7:0} {w * 0.58:0},{h * 0.42:0} {w * 0.8:0},{h * 0.7:0} {w},{h * 0.58:0} {w},{h}' fill='{t.C}' fill-opacity='.55'/>" +
                  $"<polygon points='0,{h} 0,{h * 0.86:0} {w * 0.3:0},{h * 0.66:0} {w * 0.52:0},{h * 0.86:0} {w * 0.74:0},{h * 0.7:0} {w},{h * 0.88:0} {w},{h}' fill='{t.C}' fill-opacity='.8'/></svg>";
        return "data:image/svg+xml," + Uri.EscapeDataString(svg);
    }
    // CSS background-image value (used inside style=""): same local picture, no network
    public string Bg(string kw, int n, int w = 1600, int h = 600) => $"url({Img(kw, n, w, h)})";

    // ---- Your own photos -------------------------------------------------------------------
    // Put a picture in  wwwroot/images/places/  named after the item (see Slug), e.g.
    //   Hunza Valley -> hunza-valley.jpg      Pearl Continental -> pearl-continental.jpg
    // (.jpg .jpeg .png .webp). If no file exists a clean placeholder is shown.
        static readonly string[] Ext = { ".jpg", ".jpeg", ".png", ".webp" };

    public string Slug(string name)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in name.ToLowerInvariant())
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (sb.Length > 0 && sb[^1] != '-') sb.Append('-');
        return sb.ToString().Trim('-');
    }

    // URL of your own picture for this item, or null when you have not added one yet
    public string LocalPic(Item i)
    {
        if (string.IsNullOrEmpty(WebRoot)) return null;
        var slug = Slug(i.Name);
        foreach (var e in Ext)
            if (File.Exists(Path.Combine(WebRoot, "images", "places", slug + e))) return "/images/places/" + slug + e;
        return null;
    }

    // Your own picture of a package (same naming rule as items: "Karachi Beach Weekend" -> karachi-beach-weekend.jpg)
    public string LocalPic(TourPackage p)
    {
        if (string.IsNullOrEmpty(WebRoot)) return null;
        var slug = Slug(p.Name);
        foreach (var e in Ext)
            if (File.Exists(Path.Combine(WebRoot, "images", "places", slug + e))) return "/images/places/" + slug + e;
        return null;
    }

    // Every card shows exactly ONE picture: yours (LocalPic) or, when you have not added one yet,
    // a clean placeholder with the name (views check for null). No random online photos, so nothing irrelevant is shown.

    // CSS "background-image" value for a banner: the item's own picture, or "none" (plain green banner)
    public string ItemBg(Item i) { var l = LocalPic(i); return l != null ? $"url({l})" : "none"; }
    public string PackageBg(TourPackage p) { var l = LocalPic(p); return l != null ? $"url({l})" : "none"; }
}
