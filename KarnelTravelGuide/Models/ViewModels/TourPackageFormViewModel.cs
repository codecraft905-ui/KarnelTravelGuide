using System.ComponentModel.DataAnnotations;

namespace KarnelTravelGuide.ViewModels;

// Used by the Add / Edit Tour Package form (validation rules live here, not on the database class)
public class TourPackageFormViewModel
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Please enter the package name."), StringLength(100, ErrorMessage = "The package name cannot exceed 100 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Please enter the offer (for example 20% OFF)."), StringLength(100, ErrorMessage = "The offer cannot exceed 100 characters.")]
    public string Offer { get; set; }
    [Required(ErrorMessage = "Please enter photo keywords (for example beach,sunset)."), StringLength(100, ErrorMessage = "The keywords cannot exceed 100 characters.")]
    public string Kw { get; set; }
}
