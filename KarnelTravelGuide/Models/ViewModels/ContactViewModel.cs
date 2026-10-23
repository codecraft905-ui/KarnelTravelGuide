using System.ComponentModel.DataAnnotations;

namespace KarnelTravelGuide.ViewModels;

public class ContactViewModel
{
    [Required(ErrorMessage = "Please enter your full name."), StringLength(100)] public string Name { get; set; }
    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address."), StringLength(150)] public string Email { get; set; }
    [Phone(ErrorMessage = "Please enter a valid phone number."), StringLength(30)] public string Phone { get; set; }
    [Required] public string Type { get; set; } = "Inquiry";   // Inquiry or Feedback (bookings use the Book page)
    [Range(1, 5)] public int Rating { get; set; } = 5;
    [Required(ErrorMessage = "Please write your message."), StringLength(1000, ErrorMessage = "Your message cannot exceed 1,000 characters.")] public string Message { get; set; }
    [StringLength(200)] public string Destination { get; set; }
    public DateTime? TravelDate { get; set; }
    [Range(1, 100, ErrorMessage = "The number of travelers must be between 1 and 100.")] public int People { get; set; } = 2;
    public bool Sent { get; set; }
}
