namespace KarnelTravelGuide.Models;

// One row of the TourPackages table (columns: Id, Name, Offer, Kw). Managed by the admin in Admin > Tour Packages.
// Keep this class free of EF attributes: the validation rules live in TourPackageFormViewModel.
public class TourPackageDb
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Offer { get; set; }
    public string Kw { get; set; }      // photo keywords, e.g. "beach,sunset"
}
