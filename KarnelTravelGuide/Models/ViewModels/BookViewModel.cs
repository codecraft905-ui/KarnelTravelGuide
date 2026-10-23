using System.ComponentModel.DataAnnotations;
using KarnelTravelGuide.Helpers;
using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

public class BookViewModel
{
    [Required(ErrorMessage = "Please enter your full name."), StringLength(100)] public string Name { get; set; }
    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address."), StringLength(150)] public string Email { get; set; }
    [Required(ErrorMessage = "Please enter your phone number."), Phone(ErrorMessage = "Please enter a valid phone number."), StringLength(30, MinimumLength = 7, ErrorMessage = "Please enter a valid phone number.")] public string Phone { get; set; }
    [Required(ErrorMessage = "Please choose a package.")] public string Destination { get; set; }   // the package (hotel, restaurants and places are included)
    [Required(ErrorMessage = "Please choose your preferred travel date.")] public DateTime? TravelDate { get; set; }
    [Required(ErrorMessage = "Please enter the number of travelers."), Range(1, 100, ErrorMessage = "The number of travelers must be between 1 and 100.")] public int? People { get; set; } = 2;
    [StringLength(1000, ErrorMessage = "Your message cannot exceed 1,000 characters.")] public string Message { get; set; }
    public string Website { get; set; }                 // hidden spam trap - real users leave it empty

    public Booking Done { get; set; }                   // set after a successful request
    public string MinDate => Pk.Today.ToString("yyyy-MM-dd");
}
