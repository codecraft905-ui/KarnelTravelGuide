using System.Text.Json.Serialization;

namespace KarnelTravelGuide.Models;

public enum BookingStatus { Pending, Confirmed, Rejected }

// One booking / tour request sent from the Book page and reviewed in the Admin panel
public class Booking
{
    public string AccountEmail { get; set; }       // e-mail of the logged-in account that made the request
    public string Id { get; set; }                 // booking reference shown to the customer, e.g. KTG-7K2M9Q
    public DateTime CreatedAtUtc { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Destination { get; set; }        // tour package or service name (empty when only a hotel was booked)
    public string Kind { get; set; }                // "Tour Package", "Tourist Spots", ...
    public string Location { get; set; }
    public int? FromPrice { get; set; }
    public string Hotel { get; set; }               // hotel / resort chosen separately, after the package
    public string HotelLocation { get; set; }
    public string Included { get; set; }            // snapshot of what the package included when it was booked (hotel, restaurants, places, travel)
    public int? HotelPrice { get; set; }
    [JsonIgnore] public string Title => string.IsNullOrWhiteSpace(Destination) ? Hotel : Destination;
    public DateTime TravelDate { get; set; }
    public int People { get; set; }
    public string Message { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string AdminNote { get; set; }           // optional note from admin, visible to the customer
    public DateTime? DecidedAtUtc { get; set; }

    public Booking Clone() => (Booking)MemberwiseClone();
}

